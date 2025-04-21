using System.ComponentModel;
using ModelContextProtocol.Server;

namespace DigmaMCPServer.Tools;

[McpServerToolType]
public class CodeObservabilityTool
{
    // 👇 Mark a method as an MCP tools
    [McpServerTool, Description("Get runtime issues for specific code location based on observability analysis")]
    public static async Task<string> GetCodeInsightsForEnvironment(DigmaClient client,
        [Description("The name of the method to check, must specify a specific method to check")] string codeMethod,
        [Description("The name of the class. Provide only the class name without the namespace prefix. This is mandatory input")] string codeClass,
        [Description("The namespace of this code location, this is mandatory input")] string codeNamespace,
        [Description("The environment id to check for runtime issues, this is mandatory input")] string environmentId)
    {
        var issuesdData= await client.GetInsightsView(environmentId,codeMethod,codeClass);
        return issuesdData;
    }
    
    [McpServerTool, Description("Get the list of environments and their ids where observability is being analyzed")]
    public static async Task<string> GetEnvironments(DigmaClient client)
    {
        var issuesdData= await client.GetEnvironments();
        return issuesdData;
    }
    
    [McpServerTool, Description("Get a list of the top runtime issues by analyzing how the code run in the specified environment ")]
    public static async Task<string> GetTopIssuesForEnvironment(DigmaClient client, string environmentId)
    {
        var issuesdData= await client.GetTopIssues(environmentId);
        return issuesdData;
    }
    
    
}