using System.Globalization;
using System.Text.Json.Serialization;
using DigmaSSEServer.Options;

namespace DigmaSSEServer.Authentication;

public sealed class DigmaTokenProvider
{
    private const string LoginRoute   = "/authentication/login";
    private const string RefreshRoute = "/authentication/refresh-token";
    private static readonly TimeSpan Skew = TimeSpan.FromSeconds(30);

    private readonly HttpClient _httpClient;
    private readonly string     _username;
    private readonly string     _password;

    private TokenInfo? _current;

    public DigmaTokenProvider(HttpClient httpClient, AuthOptions options)
    {
        _httpClient = httpClient;
        _username   = options.UserName;
        _password   = options.Password;
    }
    
    public async Task<string> GetTokenAsync(CancellationToken ct = default)
    {
        // Fast‑path: already have a non‑expired token
        if (IsValid(_current))
            return _current!.AccessToken;

        // Try refresh if we have a refresh token
        if (_current is { RefreshToken: var refresh })
        {
            _current = await TryRefreshAsync(refresh);
            if (_current != null)
            {
                return _current.AccessToken;
            }
        }

        // Fall back to log in
        _current = await LoginAsync();

        return _current.AccessToken;
    }

    private static bool IsValid(TokenInfo? t) =>
        t is { ExpiresAtUtc: var exp } && DateTime.UtcNow < exp - Skew;

    private async Task<TokenInfo> LoginAsync()
    {
        var body = new { username = _username, password = _password };
        var resp = await _httpClient.PostAsJsonAsync(LoginRoute, body);
        resp.EnsureSuccessStatusCode();

        var dto = await resp.Content.ReadFromJsonAsync<AuthResponse>().ConfigureAwait(false)
                  ?? throw new InvalidOperationException("Empty login payload");

        return dto.ToTokenInfo();
    }

    private async Task<TokenInfo?> TryRefreshAsync(string refreshToken)
    {
        var dtoIn = new { accessToken = _current!.AccessToken, refreshToken };
        var resp  = await _httpClient.PostAsJsonAsync(RefreshRoute, dtoIn).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode) return null;

        var dto = await resp.Content.ReadFromJsonAsync<AuthResponse>().ConfigureAwait(false);
        return dto?.ToTokenInfo();
    }

    private sealed record AuthResponse(
        [property: JsonPropertyName("accessToken")]  string AccessToken,
        [property: JsonPropertyName("refreshToken")] string RefreshToken,
        [property: JsonPropertyName("expiration")]   string Expiration)
    {
        public TokenInfo ToTokenInfo() =>
            new(AccessToken,
                DateTime.Parse(Expiration, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal)
                        .ToUniversalTime(),
                RefreshToken);
    }

    private sealed record TokenInfo(string AccessToken, DateTime ExpiresAtUtc, string RefreshToken);
}