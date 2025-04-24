# Digma MCP Server

A Model Context Protocol (MCP) server implementation for enabling agents to access observability insights using [Digma](https://digma.ai).

## Key Features 🚀

*   **🗣️ Observability-assisted code reviews:** Instruct your LLM to perform Unity tasks.
*   **🔎 Find code inefficiencies with dynamic code analysis:** Identify issues in the code/queries that are slowing the app down
*   **🔭 Utilize code runtime usage data from distributed tracing:** Check for breaking changes or generated relevant tests

## Example prompts 💬 

* `help me review the code changes in this branch by looking at related runtime issues`
* `I want to improve the performance of this app. What are the three most severe issues I can fix?`
* `I'm making changes to this function, based on runtime data. What other services and code would be affected?`
* `Are there any new issues in this code based on the Staging environment?`
* `Which database queries have the most impact on the application performance?`

---

## How It Works 🔧

Digma is an application that pre-processes your observability data to identify issues and track code performance and runtime data. 
To get started:
1.  **Deloy Digma in Your Cluster**: Digma is a K8s native application; follow this [guide](https://docs.digma.ai/digma-developer-guide/installation/central-on-prem-install) to install.
2.  **Send Digma Traces:** Digma accepts any standard OTEL traces; it's easy to extend your data pipeline to [send the observability](https://docs.digma.ai/digma-developer-guide/instrumentation/instrumenting-your-code-for-tracing) data to your local Digma deployment. Alternatively, you can [dual-ship](https://docs.digma.ai/digma-developer-guide/instrumentation/sending-data-to-digma-using-the-datadog-agent) your Datadog traces if you're using DD.
3.  **Follow the instructions below to install the Digma MCP Server with your GenAI agent**

---

## Installation ⚙️

Configure your MCP Client (Claude, Cursor, etc.) to include the Digma MCP
The Digma MCP is included as a remote SSE server in your Digma deployment. You can configure it using its URL in your client, or use an MCP tool such as [SuperGateway](https://github.com/supercorp-ai/supergateway) to run it as a command tool. 
The MCP URL path is composed of the Digma API Key as follows:
`https://<DIGMA_URL>/<DIGMA_API_TOKEN>>/sse`

### Example MCP XML 

If your client supports SSE servers, you can use the following syntax:

  ```json
  {
    "mcpServers": {
      "digma": {
      "url": "https://<DIGMA_URL>/DIGMA_API_TOKEN>/sse",
     
        }
      // ... other servers might be here ...
    }
  }
```

To use the MCP server as a command tool, use the [SuperGateway](https://github.com/supercorp-ai/supergateway) tool to bridge to the URL as seen below:

  ```json
  {
    "digma": {
      "command": "npx",
      "args": [
        "-y",
        "supergateway",
        "--sse",
        "https://<DIGMA_URL>/DIGMA_API_TOKEN>/sse"
      ]
    }
  }
```


## License 📜

MIT License. See [LICENSE](https://www.google.com/url?sa=E&q=https%3A%2F%2Fgithub.com%2Fjustinpbarnett%2Funity-mcp%2Fblob%2Fmaster%2FLICENSE) file.

