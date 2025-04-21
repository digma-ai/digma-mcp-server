using System.Net.Http.Headers;

namespace DigmaMCPServer.Auth;

class ApiRequestHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public ApiRequestHandler(ITokenProvider tokenProvider, AccessTokenRequestHandler accessTokenRequestHandler) 
        : base(accessTokenRequestHandler)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}