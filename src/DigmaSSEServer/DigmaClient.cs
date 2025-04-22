using System.Net;
using System.Text;
using System.Text.Json;

namespace DigmaSSEServer;

public class DigmaClient
{
    private readonly HttpClient _httpClient;
    
    public DigmaClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> GetTopIssues(string? environmentId, int page = 0, int pageSize = 10)
    {
        try
        {
            var request = new
            {
                environment = environmentId,
                sortBy = "criticality",
                sortOrder = "desc",
                page,
                pageSize
            };

            var response = await _httpClient.PostAsync("/mcp/top-issues", GetJsonContent(request));
            await EnsureSuccessStatusCodeWithContentAsync(response);
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to get top issues: {ex.Message}", ex);
        }
    }

    private static async Task EnsureSuccessStatusCodeWithContentAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Request failed with status code {response.StatusCode}: {content}",
                null,
                response.StatusCode);
        }
    }

    public async Task<string> GetAllIssuesForMethod(string environmentId, string className, string functionName)
    {
        var request = new
        {
            environmentId,
            methodName = functionName,
            className
        };
        var response = await _httpClient.PostAsync("/mcp/all-issues-for-method", GetJsonContent(request));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetEnvironments()
    {
        try
        {
            var response = await _httpClient.GetAsync("/mcp/environments");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {content}");
            await EnsureSuccessStatusCodeWithContentAsync(response);
            return content;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error getting environments: {ex.Message}");
            throw new Exception($"Failed to get environments: {ex.Message}", ex);
        }
    }
    
    public async Task<string> GetUsagesForMethod(string environmentId, string className, string methodName)
    {
        try
        {
            var request = new
            {
                environmentId,
                className,
                methodName
            };

            var response = await _httpClient.PostAsync("/mcp/usages-for-method", GetJsonContent(request));
            await EnsureSuccessStatusCodeWithContentAsync(response);
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to get method usages: {ex.Message}", ex);
        }
    }

    public async Task<string> GetTrace(string traceId)
    {
        var response = await _httpClient.GetAsync($"/mcp/trace/{traceId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetAssetsByCategory(string environmentId, string? category = "", int page = 0, int pageSize = 10)
    {
        var request = new
        {
            environmentId,
            category = string.IsNullOrEmpty(category) ? null : category,
            page,
            pageSize
        };
        var response = await _httpClient.PostAsync("/mcp/assets", GetJsonContent(request));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetAssetCategories(string environmentId)
    {
        var encodedEnv = WebUtility.UrlEncode(environmentId);
        var response = await _httpClient.GetAsync($"/mcp/assets-categories?environment={encodedEnv}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private static StringContent GetJsonContent(object request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }
}