using System.ComponentModel;
using ModelContextProtocol.Server;

namespace DigmaSSEServer.Tools;

[McpServerToolType]
public class CodeObservabilityTool
{
    [McpServerTool, Description("Get runtime issues for specific code location based on observability analysis")]
    public static async Task<string> GetCodeInsightsForEnvironment(DigmaClient client,
        [Description("The name of the method to check, must specify a specific method to check")] string codeMethod,
        [Description("The name of the class. Provide only the class name without the namespace prefix. This is mandatory input")] string codeClass,
        [Description("The namespace of this code location, this is mandatory input")] string codeNamespace,
        [Description("The environment id to check for runtime issues, this is mandatory input")] string environmentId)
    {
        return await client.GetAllIssuesForMethod(environmentId, codeClass, codeMethod);
    }
    
    [McpServerTool, Description("Get the list of environments and their ids where observability is being analyzed")]
    public static async Task<string> GetEnvironments(DigmaClient client)
    {
        return await client.GetEnvironments();
    }
    
    [McpServerTool, Description("Get a list of the top runtime issues by analyzing how the code run on specific environment")]
    public static async Task<string> GetTopIssuesByEnvironment(DigmaClient client,
        [Description("The environment id to check for top issues")] string environmentId,
        [Description("Page number (zero-based)")] int page = 0,
        [Description("Number of items per page")] int pageSize = 10)
    {
        return await client.GetTopIssues(environmentId, page, pageSize);
    }
    
    [McpServerTool, Description("Get a list of the top runtime issues by analyzing how the code run for all environment")]
    public static async Task<string> GetTopIssues(DigmaClient client,
        [Description("Page number (zero-based)")] int page = 0,
        [Description("Number of items per page")] int pageSize = 10)
    {
        return await client.GetTopIssues(null, page, pageSize);
    }
    
    [McpServerTool, Description("Get usage insights for a specific method based on observability analysis")]
    public static async Task<string> GetUsagesForMethod(DigmaClient client,
        [Description("The environment id to check for usages")] string environmentId,
        [Description("The name of the class. Provide only the class name without the namespace prefix.")] string codeClass,
        [Description("The name of the method to check, must specify a specific method to check")] string codeMethod)
    {
        return await client.GetUsagesForMethod(environmentId, codeClass, codeMethod);
    }
    
    [McpServerTool, Description("Get the list of asset categories by environment available for performance insights based on observability analysis")]
    public static async Task<string> GetAssetCategories(DigmaClient client, 
        [Description("The environment id to get assets cetegories")] string environmentId)
    {
        return await client.GetAssetCategories(environmentId);
    }
    
    [McpServerTool, Description("Get the execution trace for a specific runtime issue based on observability analysis")]
    public static async Task<string> GetTrace(DigmaClient client, 
        [Description("The trace id to get trace json")] string traceId)
    {
        return await client.GetTrace(traceId);
    }
    
    [McpServerTool, Description("Get code assets filtered by performance impact and category based on observability analysis")]
    public static async Task<string> GetAssetsByCategory(DigmaClient client,
        [Description("The environment id to get assets categories")] string environmentId,
        [Description("Asset category to filter assets. Optional")] string? assetCategory = null,
        [Description("Page number (zero-based)")] int page = 0,
        [Description("Number of items per page")] int pageSize = 10)
    {
        return await client.GetAssetsByCategory(environmentId, assetCategory, page, pageSize);
    }
}