using System.Net.Http.Headers;

namespace DigmaSSEServer.Authentication;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly DigmaTokenProvider _tokenProvider;

    public BearerTokenHandler(DigmaTokenProvider tokenProvider, HttpMessageHandler innerHandler = null)
    {
        _tokenProvider = tokenProvider;
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}