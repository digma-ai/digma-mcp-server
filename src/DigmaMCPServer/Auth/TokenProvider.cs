using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace DigmaMCPServer.Auth;

public interface ITokenProvider
{
    Task<string> GetTokenAsync();
}
public class TokenProvider: ITokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<AuthOptions> _options;
    private readonly ILogger<TokenProvider> _logger;
    private DateTime _expiresAt;

    private string? _token;

    public TokenProvider(HttpClient httpClient, IOptions<AuthOptions> options, ILogger<TokenProvider> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    public async Task<string> GetTokenAsync()
    {
        if (string.IsNullOrEmpty(_token) || DateTime.UtcNow >= _expiresAt)
        {
            var result = await LoginAsync();
            _token = result.token;
            _expiresAt = result.expiresAt;
        }
        return _token;
    }

    private async Task<(string token, DateTime expiresAt)> LoginAsync()
    {
        var options = _options.Value;
        
        var payload = new
        {
            username = options.Email,
            password = options.Password
        };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        _logger.LogInformation(_httpClient.BaseAddress.AbsoluteUri);
        var response = await _httpClient.PostAsync("/authentication/login", content);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var contentStr = await response.Content.ReadAsStringAsync();
            _logger.LogError(contentStr);
        }
        response.EnsureSuccessStatusCode();
        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var resultJson = await JsonDocument.ParseAsync(responseStream);
        var token = resultJson.RootElement.GetProperty("accessToken").GetString();

        // Optional: Add logic to parse token expiration if available
        var expiresAt = DateTime.UtcNow.AddMinutes(55); // Assuming 1 hour token
        return (token!, expiresAt);
    }
}