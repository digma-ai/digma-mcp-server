using System.Net;
using System.Text;
using System.Text.Json;

namespace DigmaMCPServer;

public class DigmaClient
{
    private readonly HttpClient _httpClient;

    public DigmaClient(HttpClient httpClient)
    {  
        _httpClient = httpClient;
    }

    public async Task<string> GetEnvironments( )
    {
        var response = await _httpClient.GetAsync($"/Environments");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<string> GetInsightsView(string environmentId, string functionName, string className)
    {
        string encodedEnv = WebUtility.UrlEncode(environmentId);

        var encodedSpanCodeObjectId =
            WebUtility.UrlEncode(
                $"span:io.opentelemetry.opentelemetry-instrumentation-annotations-1.16$_${className}.{functionName}");

        var parameters = $"?Page=1&PageSize=10&ShowDismissed=true&ShowUnreadOnly=false&Environment={encodedEnv}&ScopedSpanCodeObjectId={encodedSpanCodeObjectId}";
        var requestUri = $"/Insights/get_insights_view/{parameters}";
        Console.Out.WriteLine(requestUri);
        var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }


    public async Task<string> GetTopIssues(string environmentId)
    {
        var requestContent = new
        {
            environment = environmentId,
            sortBy = "criticality",
            sortOrder = "desc"
        };
        string json = JsonSerializer.Serialize(requestContent);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var requestUri = $"/Insights/issues";
        Console.Out.WriteLine(requestUri);
        
        var response = await _httpClient.PostAsync(requestUri,content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}