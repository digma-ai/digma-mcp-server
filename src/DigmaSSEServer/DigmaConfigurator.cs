using DigmaSSEServer.Authentication;
using DigmaSSEServer.Options;
using Microsoft.Extensions.Options;

namespace DigmaSSEServer;

public static class DigmaConfigurator
{
    private const string TokenClientName = "DigmaTokenClient";

    public static IServiceCollection AddDigma(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<BearerTokenHandler>();

        services.Configure<DigmaOptions>(configuration.GetSection("Digma"));
        services.Configure<AuthOptions>(configuration.GetSection("Auth"));

        static HttpClientHandler CreateHandler()
        {
            var handler = new HttpClientHandler();
#if DEBUG
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif
            return handler;
        }

        services.AddHttpClient(TokenClientName, ConfigureHttpClient)
            .ConfigurePrimaryHttpMessageHandler(CreateHandler);

        services.AddHttpClient<DigmaClient>(ConfigureHttpClient)
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var delegating = sp.GetRequiredService<BearerTokenHandler>();
                delegating.InnerHandler = CreateHandler();
                return delegating;
            });

        services.AddSingleton<DigmaTokenProvider>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(TokenClientName);
            var auth = sp.GetRequiredService<IOptions<AuthOptions>>().Value;
            return new DigmaTokenProvider(http, auth);
        });

        return services;
    }

    private static void ConfigureHttpClient(IServiceProvider sp, HttpClient c)
    {
        var digma = sp.GetRequiredService<IOptions<DigmaOptions>>().Value;
        var auth = sp.GetRequiredService<IOptions<AuthOptions>>().Value;

        c.BaseAddress = new Uri(digma.AnalyticsApi);
        c.DefaultRequestHeaders.Add("Digma-Access-Token", $"Token {auth.Token}");
    }
}