using DigmaSSEServer;
using DigmaSSEServer.Authentication;
using DigmaSSEServer.Options;
using DigmaSSEServer.Tools;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Protocol.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDigma(builder.Configuration);
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new Implementation()
        {
            Name = "Digma Server",
            Version = "1.0"
            
        };
    })
    .WithHttpTransport() // OR WithStdioServerTransport()
    .WithTools<CodeObservabilityTool>();

var app = builder.Build();
var authOptions = app.Services.GetRequiredService<IOptions<AuthOptions>>();
if (string.IsNullOrWhiteSpace(authOptions.Value.Token))
{
    Console.WriteLine("ERROR: Missing required configuration 'Auth__Token must be specified'");
    Environment.Exit(1); // Exit with failure code
    return;
}

app.MapMcp(Constants.RoutePatternPrefix);


app.UseMiddleware<ApiKeyValidatorMiddleware>();

app.Run();