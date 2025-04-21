using System.Net;
using DigmaSSEServer;
using DigmaSSEServer.Tools;
using ModelContextProtocol.Protocol.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDigma(builder.Configuration);

builder.Services
    .AddMcpServer(options => options.ServerInfo = new Implementation()
    {
        Name = "Digma Server",
        Version = "1.0"
    })
    .WithHttpTransport() // OR WithStdioServerTransport()
    .WithTools<CodeObservabilityTool>();

var app = builder.Build();

app.MapMcp();

app.Run();