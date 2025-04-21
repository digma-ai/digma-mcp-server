using Microsoft.Extensions.Options;

namespace DigmaMCPServer.Auth;

class AccessTokenRequestHandler : DelegatingHandler
{
    private readonly IOptions<AuthOptions> _options;

    public AccessTokenRequestHandler(IOptions<AuthOptions> options) 
        : base(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        })
    {
        _options = options;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var apiToken = _options.Value.ApiToken;
        if (!string.IsNullOrEmpty(apiToken)) request.Headers.Add("Digma-Access-Token", "Token " + apiToken);
        return await base.SendAsync(request, cancellationToken);
    }
}