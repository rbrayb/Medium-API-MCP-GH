using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Medium.Demos.ConsoleApp.MCP
{
    /// <summary>
    /// MCP Protocol handler implementing the Model Context Protocol specification
    /// Communicates via stdio (stdin/stdout) for integration with GitHub Copilot
    /// </summary>
    public class McpProtocolHandler
    {
        private readonly IMcpToolHandler _toolHandler;
        private readonly ILogger<McpProtocolHandler> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public McpProtocolHandler(IMcpToolHandler toolHandler, ILogger<McpProtocolHandler> logger)
        {
            _toolHandler = toolHandler;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };
        }

        /// <summary>
        /// Start the MCP server and handle stdio communication
        /// </summary>
        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("========================================");
            _logger.LogDebug("Medium MCP Server started. Listening for requests...");
            _logger.LogDebug("Protocol Version: 2024-11-05");
            _logger.LogDebug("Server Name: medium-stats");
            _logger.LogDebug("Server Version: 1.0.0");
            _logger.LogDebug("========================================");

            // Send server info on startup
            await SendServerInfoAsync();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var line = await Console.In.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    _logger.LogDebug("========================================");
                    _logger.LogDebug("RAW REQUEST RECEIVED:");
                    _logger.LogDebug("{RequestJson}", line);
                    _logger.LogDebug("========================================");

                    try
                    {
                        var request = JsonSerializer.Deserialize<McpRequest>(line, _jsonOptions);
                        if (request != null)
                        {
                            _logger.LogDebug(">>> INCOMING REQUEST: Method='{Method}', ID='{RequestId}'", 
                                request.Method, request.Id);
                            
                            if (request.Params != null && request.Params.Count > 0)
                            {
                                _logger.LogDebug("Request Parameters:");
                                foreach (var param in request.Params)
                                {
                                    _logger.LogDebug("  {Key} = {Value}", param.Key, JsonSerializer.Serialize(param.Value));
                                }
                            }

                            await HandleRequestAsync(request);
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "!!! JSON PARSE ERROR !!!");
                        _logger.LogError("Failed to parse request: {Line}", line);
                        await SendErrorAsync("parse_error", "Invalid JSON request");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "!!! FATAL ERROR in MCP Server !!!");
                throw;
            }
        }

        private async Task SendServerInfoAsync()
        {
            var serverInfo = new McpServerInfo
            {
                ProtocolVersion = "2024-11-05",
                ServerInfo = new ServerInfoDetails
                {
                    Name = "medium-stats",
                    Version = "1.0.0"
                },
                Capabilities = new ServerCapabilities
                {
                    Tools = new ToolsCapability()
                }
            };

            _logger.LogDebug(">>> SENDING SERVER INFO (Initialization)");
            _logger.LogDebug("Server Info: {ServerInfo}", JsonSerializer.Serialize(serverInfo, new JsonSerializerOptions { WriteIndented = true }));
            
            await SendResponseAsync(serverInfo);
        }

        private async Task HandleRequestAsync(McpRequest request)
        {
            try
            {
                _logger.LogDebug("=== PROCESSING REQUEST: {Method} ===", request.Method);

                // Check if this is a notification (no ID means no response expected)
                var isNotification = request.Id == null;
                if (isNotification)
                {
                    _logger.LogDebug("(Notification - no response expected)");
                }

                switch (request.Method)
                {
                    case "initialize":
                        _logger.LogDebug("Handling INITIALIZE request");
                        await HandleInitializeAsync(request);
                        break;

                    case "notifications/initialized":
                        _logger.LogDebug("Handling INITIALIZED notification");
                        await HandleInitializedNotificationAsync(request);
                        break;

                    case "tools/list":
                        _logger.LogDebug("Handling TOOLS/LIST request (Tool Discovery)");
                        await HandleToolsListAsync(request);
                        break;

                    case "tools/call":
                        _logger.LogDebug("Handling TOOLS/CALL request (Tool Execution)");
                        await HandleToolCallAsync(request);
                        break;

                    case "ping":
                        _logger.LogDebug("Handling PING request");
                        await HandlePingAsync(request);
                        break;

                    default:
                        // Only send error for unknown methods if it's not a notification
                        if (isNotification)
                        {
                            _logger.LogWarning("? Unknown notification (ignored): {Method}", request.Method);
                        }
                        else
                        {
                            _logger.LogWarning("!!! UNKNOWN METHOD: {Method} !!!", request.Method);
                            await SendErrorResponseAsync(request.Id, "method_not_found", $"Unknown method: {request.Method}");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "!!! ERROR processing request: {Method} !!!", request.Method);
                _logger.LogError("Exception Type: {ExceptionType}", ex.GetType().Name);
                _logger.LogError("Exception Message: {Message}", ex.Message);
                _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
                
                // Only send error response if this is a request (not a notification)
                if (request.Id != null)
                {
                    await SendErrorResponseAsync(request.Id, "internal_error", ex.Message);
                }
            }
        }

        private Task HandleInitializedNotificationAsync(McpRequest request)
        {
            _logger.LogDebug("--- INITIALIZED NOTIFICATION ---");
            _logger.LogDebug("Client has completed initialization");
            
            if (request.Params != null && request.Params.Count > 0)
            {
                _logger.LogDebug("Notification Parameters: {Params}", 
                    JsonSerializer.Serialize(request.Params, new JsonSerializerOptions { WriteIndented = true }));
            }
            
            _logger.LogDebug("? Initialized notification acknowledged (no response sent)");
            
            // Notifications don't require a response
            return Task.CompletedTask;
        }

        private async Task HandleInitializeAsync(McpRequest request)
        {
            _logger.LogDebug("--- INITIALIZE REQUEST ---");
            _logger.LogDebug("Request ID: {RequestId}", request.Id);
            
            if (request.Params != null)
            {
                _logger.LogDebug("Client Capabilities: {Params}", JsonSerializer.Serialize(request.Params, new JsonSerializerOptions { WriteIndented = true }));
            }

            var response = new McpResponse
            {
                Jsonrpc = "2.0",
                Id = request.Id,
                Result = new InitializeResult
                {
                    ProtocolVersion = "2024-11-05",
                    ServerInfo = new ServerInfoDetails
                    {
                        Name = "medium-stats",
                        Version = "1.0.0"
                    },
                    Capabilities = new ServerCapabilities
                    {
                        Tools = new ToolsCapability()
                    }
                }
            };

            _logger.LogDebug("<<< SENDING INITIALIZE RESPONSE");
            _logger.LogDebug("Response: {Response}", JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
            
            await SendResponseAsync(response);
            
            _logger.LogDebug("? Initialization complete");
        }

        private async Task HandleToolsListAsync(McpRequest request)
        {
            _logger.LogDebug("--- TOOLS LIST REQUEST (Tool Discovery) ---");
            _logger.LogDebug("Request ID: {RequestId}", request.Id);

            var tools = _toolHandler.GetToolDefinitions();
            
            _logger.LogDebug("Discovered {ToolCount} tools:", tools.Count);
            foreach (var tool in tools)
            {
                _logger.LogDebug("  • {ToolName}: {Description}", tool.Name, tool.Description);
                _logger.LogDebug("    Parameters: {ParamCount}", tool.Parameters.Count);
                foreach (var param in tool.Parameters)
                {
                    _logger.LogDebug("      - {ParamName} ({Type}): {Description} [Required: {Required}]", 
                        param.Key, param.Value.Type, param.Value.Description, param.Value.Required);
                }
            }

            var mcpTools = tools.Select(t => new McpTool
            {
                Name = t.Name,
                Description = t.Description,
                InputSchema = new ToolInputSchema
                {
                    Type = "object",
                    Properties = t.Parameters.ToDictionary(
                        p => p.Key,
                        p => new ToolProperty
                        {
                            Type = p.Value.Type,
                            Description = p.Value.Description
                        }
                    ),
                    Required = t.Parameters.Where(p => p.Value.Required).Select(p => p.Key).ToList()
                }
            }).ToList();

            var response = new McpResponse
            {
                Jsonrpc = "2.0",
                Id = request.Id,
                Result = new ToolsListResult
                {
                    Tools = mcpTools
                }
            };

            _logger.LogDebug("<<< SENDING TOOLS LIST RESPONSE");
            _logger.LogDebug("Response: {Response}", JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
            
            await SendResponseAsync(response);
            
            _logger.LogDebug("? Tool discovery complete");
        }

        private async Task HandleToolCallAsync(McpRequest request)
        {
            _logger.LogDebug("--- TOOL CALL REQUEST (Tool Execution) ---");
            _logger.LogDebug("Request ID: {RequestId}", request.Id);

            if (request.Params?.TryGetValue("name", out var nameObj) != true)
            {
                _logger.LogError("!!! Missing 'name' parameter in tool call !!!");
                await SendErrorResponseAsync(request.Id, "invalid_params", "Missing 'name' parameter");
                return;
            }

            var toolName = nameObj?.ToString() ?? string.Empty;
            _logger.LogDebug("Executing Tool: '{ToolName}'", toolName);
            
            var parameters = new Dictionary<string, object>();
            if (request.Params.TryGetValue("arguments", out var argsObj) && argsObj is JsonElement argsElement)
            {
                parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(argsElement.GetRawText(), _jsonOptions) 
                    ?? new Dictionary<string, object>();
                
                _logger.LogDebug("Tool Arguments:");
                foreach (var param in parameters)
                {
                    _logger.LogDebug("  {Key} = {Value}", param.Key, param.Value);
                }
            }
            else
            {
                _logger.LogDebug("No arguments provided for tool call");
            }

            _logger.LogDebug("Calling tool handler for '{ToolName}'...", toolName);
            var startTime = DateTime.UtcNow;
            
            var result = await _toolHandler.HandleToolCallAsync(toolName, parameters);
            
            var duration = DateTime.UtcNow - startTime;
            _logger.LogDebug("? Tool execution completed in {Duration}ms", duration.TotalMilliseconds);
            _logger.LogDebug("Tool Result: {Result}", result);

            var response = new McpResponse
            {
                Jsonrpc = "2.0",
                Id = request.Id,
                Result = new ToolCallResult
                {
                    Content = new List<ContentItem>
                    {
                        new ContentItem
                        {
                            Type = "text",
                            Text = result
                        }
                    }
                }
            };

            _logger.LogDebug("<<< SENDING TOOL CALL RESPONSE");
            _logger.LogDebug("Response: {Response}", JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
            
            await SendResponseAsync(response);
            
            _logger.LogDebug("? Tool call complete");
        }

        private async Task HandlePingAsync(McpRequest request)
        {
            _logger.LogDebug("--- PING REQUEST ---");
            _logger.LogDebug("Request ID: {RequestId}", request.Id);

            var response = new McpResponse
            {
                Jsonrpc = "2.0",
                Id = request.Id,
                Result = new { }
            };

            _logger.LogDebug("<<< SENDING PING RESPONSE");
            
            await SendResponseAsync(response);
            
            _logger.LogDebug("? Ping complete");
        }

        private async Task SendResponseAsync(object response)
        {
            var json = JsonSerializer.Serialize(response, _jsonOptions);
            
            _logger.LogDebug("----------------------------------------");
            _logger.LogDebug("RAW RESPONSE SENT:");
            _logger.LogDebug("{ResponseJson}", json);
            _logger.LogDebug("----------------------------------------");
            
            await Console.Out.WriteLineAsync(json);
            await Console.Out.FlushAsync();
            
            _logger.LogDebug("? Response flushed to stdout");
        }

        private async Task SendErrorResponseAsync(object? id, string code, string message)
        {
            _logger.LogError("!!! SENDING ERROR RESPONSE !!!");
            _logger.LogError("Error Code: {Code}", code);
            _logger.LogError("Error Message: {Message}", message);
            _logger.LogError("Request ID: {RequestId}", id);

            var response = new McpResponse
            {
                Jsonrpc = "2.0",
                Id = id,
                Error = new McpError
                {
                    Code = code,
                    Message = message
                }
            };

            _logger.LogDebug("Error Response: {Response}", JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
            
            await SendResponseAsync(response);
        }

        private async Task SendErrorAsync(string code, string message)
        {
            _logger.LogError("!!! SENDING ERROR NOTIFICATION (No Request ID) !!!");
            await SendErrorResponseAsync(null, code, message);
        }
    }

    // MCP Protocol Models
    public class McpRequest
    {
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; } = "2.0";

        [JsonPropertyName("id")]
        public object? Id { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;

        [JsonPropertyName("params")]
        public Dictionary<string, object>? Params { get; set; }
    }

    public class McpResponse
    {
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; } = "2.0";

        [JsonPropertyName("id")]
        public object? Id { get; set; }

        [JsonPropertyName("result")]
        public object? Result { get; set; }

        [JsonPropertyName("error")]
        public McpError? Error { get; set; }
    }

    public class McpError
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class McpServerInfo
    {
        [JsonPropertyName("protocolVersion")]
        public string ProtocolVersion { get; set; } = string.Empty;

        [JsonPropertyName("serverInfo")]
        public ServerInfoDetails ServerInfo { get; set; } = new();

        [JsonPropertyName("capabilities")]
        public ServerCapabilities Capabilities { get; set; } = new();
    }

    public class ServerInfoDetails
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;
    }

    public class ServerCapabilities
    {
        [JsonPropertyName("tools")]
        public ToolsCapability? Tools { get; set; }
    }

    public class ToolsCapability
    {
    }

    public class InitializeResult
    {
        [JsonPropertyName("protocolVersion")]
        public string ProtocolVersion { get; set; } = string.Empty;

        [JsonPropertyName("serverInfo")]
        public ServerInfoDetails ServerInfo { get; set; } = new();

        [JsonPropertyName("capabilities")]
        public ServerCapabilities Capabilities { get; set; } = new();
    }

    public class ToolsListResult
    {
        [JsonPropertyName("tools")]
        public List<McpTool> Tools { get; set; } = new();
    }

    public class McpTool
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("inputSchema")]
        public ToolInputSchema InputSchema { get; set; } = new();
    }

    public class ToolInputSchema
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "object";

        [JsonPropertyName("properties")]
        public Dictionary<string, ToolProperty> Properties { get; set; } = new();

        [JsonPropertyName("required")]
        public List<string> Required { get; set; } = new();
    }

    public class ToolProperty
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    public class ToolCallResult
    {
        [JsonPropertyName("content")]
        public List<ContentItem> Content { get; set; } = new();
    }

    public class ContentItem
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}
