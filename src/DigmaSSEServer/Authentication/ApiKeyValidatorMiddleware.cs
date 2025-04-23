using DigmaSSEServer.Options;
using Microsoft.Extensions.Options;

namespace DigmaSSEServer.Authentication;

public class ApiKeyValidatorMiddleware
{ 
    private readonly RequestDelegate _next;
    private readonly string _token;
    const string INVALID_API_KEY = "unauthorized access invalid api key";

    public ApiKeyValidatorMiddleware(RequestDelegate rd, IOptions<AuthOptions> options)
    {
        _next = rd;
        _token = options.Value.Token;
    }
    private async Task UnauthorizedAccess(HttpContext httpContext, string errorMessage)
    {
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await httpContext.Response.WriteAsync(errorMessage);
    }
    
    public async Task Invoke(HttpContext httpContext)
    {
        var routeValue = httpContext.GetRouteValue(Constants.API_KEY_NAME);
        if (routeValue == null) await UnauthorizedAccess(httpContext,INVALID_API_KEY);
        var apiKey = (string)routeValue!;
        if (apiKey != _token)
        {
            await UnauthorizedAccess(httpContext,INVALID_API_KEY);
            return;
        }

        
        await _next.Invoke(httpContext);
    }
}