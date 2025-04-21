using DigmaMCPServer;
using DigmaMCPServer.Auth;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;
var serverUrl = builder.Configuration["ServerUrl"];
if (string.IsNullOrWhiteSpace(serverUrl)) throw new InvalidOperationException("ServerUrl configuration is missing. Please check appsettings.json or environment variables.");

services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));
services.AddTransient<ApiRequestHandler>();
services.AddTransient<AccessTokenRequestHandler>();
builder.Services.AddHttpClient<ITokenProvider, TokenProvider>(client => 
{
    client.BaseAddress = new Uri(serverUrl);
}).ConfigurePrimaryHttpMessageHandler<AccessTokenRequestHandler>();

builder.Services.AddHttpClient<DigmaClient>(client =>
    {
        client.BaseAddress = new Uri(serverUrl);
    })
    .ConfigurePrimaryHttpMessageHandler<ApiRequestHandler>();

builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var host = builder.Build();

Console.WriteLine("Starting Digma MCP server...");
host.Run();