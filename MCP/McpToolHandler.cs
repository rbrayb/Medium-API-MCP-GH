using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Medium.Demos.ConsoleApp.MCP
{
    /// <summary>
    /// Tool handler interface for MCP commands
    /// </summary>
    public interface IMcpToolHandler
    {
        Task<string> HandleToolCallAsync(string toolName, Dictionary<string, object> parameters);
        List<McpToolDefinition> GetToolDefinitions();
    }

    /// <summary>
    /// Handles MCP tool calls and routes them to the appropriate McpServer methods
    /// </summary>
    public class McpToolHandler : IMcpToolHandler
    {
        private readonly McpServer _mcpServer;
        private readonly ILogger<McpToolHandler> _logger;

        public McpToolHandler(McpServer mcpServer, ILogger<McpToolHandler> logger)
        {
            _mcpServer = mcpServer;
            _logger = logger;
        }

        public List<McpToolDefinition> GetToolDefinitions()
        {
            _logger.LogDebug("=== BUILDING TOOL DEFINITIONS ===");
            
            var definitions = new List<McpToolDefinition>
            {
                new McpToolDefinition
                {
                    Name = "get_blog_statistics",
                    Description = "Get comprehensive blog statistics for a Medium user including follower count, article count, bio, and social links",
                    // TODO: Update username
                    Parameters = new()
                    {
                        { "username", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Medium username (e.g., 'jbloggs')", 
                                Required = true 
                            }
                        }
                    }
                },
                new McpToolDefinition
                {
                    Name = "get_article_details",
                    Description = "Get detailed information about a specific article including claps, responses, voters, tags, and publication date",
                    Parameters = new()
                    {
                        { "article_id", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Medium article ID", 
                                Required = true 
                            }
                        }
                    }
                },
                new McpToolDefinition
                {
                    Name = "get_user_articles",
                    Description = "Get all articles for a Medium user with optional limit",
                    Parameters = new()
                    {
                        { "username", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Medium username", 
                                Required = true 
                            }
                        },
                        { "limit", new McpToolParameter 
                            { 
                                Type = "integer", 
                                Description = "Maximum number of articles to return (optional)", 
                                Required = false 
                            }
                        }
                    }
                },
                new McpToolDefinition
                {
                    Name = "search_articles",
                    Description = "Search for Medium articles by query string",
                    Parameters = new()
                    {
                        { "query", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Search query (e.g., 'Verifiable credentials', 'Azure AD')", 
                                Required = true 
                            }
                        },
                        { "limit", new McpToolParameter 
                            { 
                                Type = "integer", 
                                Description = "Maximum number of results to return (optional)", 
                                Required = false 
                            }
                        }
                    }
                },
                new McpToolDefinition
                {
                    Name = "search_tags",
                    Description = "Search for Medium tags and get information about article and author counts",
                    Parameters = new()
                    {
                        { "query", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Tag search query (e.g., 'Custom policies', 'Entra External ID')", 
                                Required = true 
                            }
                        }
                    }
                },
                new McpToolDefinition
                {
                    Name = "get_top_articles_by_claps",
                    Description = "Get the top performing articles for a user ranked by number of claps",
                    Parameters = new()
                    {
                        { "username", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Medium username", 
                                Required = true 
                            }
                        },
                        { "top_count", new McpToolParameter 
                            { 
                                Type = "integer", 
                                Description = "Number of top articles to return (default: 10)", 
                                Required = false 
                            }
                        }
                    }
                },
                new McpToolDefinition
                {
                    Name = "get_engagement_metrics",
                    Description = "Get comprehensive engagement metrics for a user's articles including total and average claps, responses, and voters",
                    Parameters = new()
                    {
                        { "username", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Medium username", 
                                Required = true 
                            }
                        }
                    }
                },
                new McpToolDefinition
                {
                    Name = "get_publication_info",
                    Description = "Get publication information including name, tagline, description, followers, tags, and social media links",
                    Parameters = new()
                    {
                        { "publication_id", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Medium publication ID", 
                                Required = true 
                            }
                        }
                    }
                },
                // ===== PHASE 1: CRITICAL FEATURES =====
                new McpToolDefinition
                {
                    Name = "get_article_content",
                    Description = "Get full article content in markdown, HTML, or plain text format. Enables content analysis, archiving, and full-text processing.",
                    Parameters = new()
                    {
                        { "article_id", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Medium article ID", 
                                Required = true 
                            }
                        },
                        { "format", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Content format: 'markdown' (default), 'html', or 'text'", 
                                Required = false 
                            }
                        }
                    }
                },
                new McpToolDefinition
                {
                    Name = "get_user_info_by_id",
                    Description = "Get user information by user ID. Faster than username lookup when you already have the user ID.",
                    Parameters = new()
                    {
                        { "user_id", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Medium user ID", 
                                Required = true 
                            }
                        }
                    }
                },
                new McpToolDefinition
                {
                    Name = "get_publication_articles",
                    Description = "Get all articles published in a specific Medium publication. Accepts either publication slug (name) or publication ID.",
                    Parameters = new()
                    {
                        { "publication_slug_or_id", new McpToolParameter 
                            { 
                                Type = "string", 
                                Description = "Publication slug (e.g., 'towards-data-science') or publication ID", 
                                Required = true 
                            }
                        },
                        { "limit", new McpToolParameter 
                            { 
                                Type = "integer", 
                                Description = "Maximum number of articles to return (optional)", 
                                Required = false 
                            }
                        }
                    }
                }
            };

            _logger.LogDebug("? Built {Count} tool definitions", definitions.Count);
            return definitions;
        }

        public async Task<string> HandleToolCallAsync(string toolName, Dictionary<string, object> parameters)
        {
            _logger.LogInformation("????????????????????????????????????????");
            _logger.LogInformation("? TOOL EXECUTION: {ToolName}", toolName);
            _logger.LogInformation("????????????????????????????????????????");

            try
            {
                _logger.LogDebug("Parsed parameters:");
                foreach (var param in parameters)
                {
                    _logger.LogDebug("  • {Key}: {Value} (Type: {Type})", 
                        param.Key, param.Value, param.Value?.GetType().Name ?? "null");
                }

                switch (toolName.ToLowerInvariant())
                {
                    case "get_blog_statistics":
                        _logger.LogInformation("? Executing: GetBlogStatisticsAsync");
                        var username = GetStringParameter(parameters, "username");
                        _logger.LogDebug("  Username: {Username}", username);
                        
                        var blogStats = await _mcpServer.GetBlogStatisticsAsync(username);
                        var blogStatsJson = JsonSerializer.Serialize(blogStats, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? GetBlogStatisticsAsync completed");
                        _logger.LogDebug("Result: {Result}", blogStatsJson);
                        return blogStatsJson;

                    case "get_article_details":
                        _logger.LogInformation("? Executing: GetArticleDetailsAsync");
                        var articleId = GetStringParameter(parameters, "article_id");
                        _logger.LogDebug("  Article ID: {ArticleId}", articleId);
                        
                        var articleDetails = await _mcpServer.GetArticleDetailsAsync(articleId);
                        var articleDetailsJson = JsonSerializer.Serialize(articleDetails, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? GetArticleDetailsAsync completed");
                        _logger.LogDebug("Result: {Result}", articleDetailsJson);
                        return articleDetailsJson;

                    case "get_user_articles":
                        _logger.LogInformation("? Executing: GetUserArticlesAsync");
                        var userArticlesUsername = GetStringParameter(parameters, "username");
                        var limit = GetIntParameter(parameters, "limit");
                        _logger.LogDebug("  Username: {Username}", userArticlesUsername);
                        _logger.LogDebug("  Limit: {Limit}", limit?.ToString() ?? "none");
                        
                        var userArticles = await _mcpServer.GetUserArticlesAsync(userArticlesUsername, limit);
                        var userArticlesJson = JsonSerializer.Serialize(userArticles, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? GetUserArticlesAsync completed");
                        _logger.LogDebug("Result: {Result}", userArticlesJson);
                        return userArticlesJson;

                    case "search_articles":
                        _logger.LogInformation("? Executing: SearchArticlesAsync");
                        var query = GetStringParameter(parameters, "query");
                        var searchLimit = GetIntParameter(parameters, "limit");
                        _logger.LogDebug("  Query: {Query}", query);
                        _logger.LogDebug("  Limit: {Limit}", searchLimit?.ToString() ?? "none");
                        
                        var searchResults = await _mcpServer.SearchArticlesAsync(query, searchLimit);
                        var searchResultsJson = JsonSerializer.Serialize(searchResults, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? SearchArticlesAsync completed");
                        _logger.LogDebug("Result: {Result}", searchResultsJson);
                        return searchResultsJson;

                    case "search_tags":
                        _logger.LogInformation("? Executing: SearchTagsAsync");
                        var tagQuery = GetStringParameter(parameters, "query");
                        _logger.LogDebug("  Query: {Query}", tagQuery);
                        
                        var tagResults = await _mcpServer.SearchTagsAsync(tagQuery);
                        var tagResultsJson = JsonSerializer.Serialize(tagResults, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? SearchTagsAsync completed");
                        _logger.LogDebug("Result: {Result}", tagResultsJson);
                        return tagResultsJson;

                    case "get_top_articles_by_claps":
                        _logger.LogInformation("? Executing: GetTopArticlesByClapsAsync");
                        var topUsername = GetStringParameter(parameters, "username");
                        var topCount = GetIntParameter(parameters, "top_count") ?? 10;
                        _logger.LogDebug("  Username: {Username}", topUsername);
                        _logger.LogDebug("  Top Count: {TopCount}", topCount);
                        
                        var topArticles = await _mcpServer.GetTopArticlesByClapsAsync(topUsername, topCount);
                        var topArticlesJson = JsonSerializer.Serialize(topArticles, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? GetTopArticlesByClapsAsync completed");
                        _logger.LogDebug("Result: {Result}", topArticlesJson);
                        return topArticlesJson;

                    case "get_engagement_metrics":
                        _logger.LogInformation("? Executing: GetEngagementMetricsAsync");
                        var metricsUsername = GetStringParameter(parameters, "username");
                        _logger.LogDebug("  Username: {Username}", metricsUsername);
                        
                        var metrics = await _mcpServer.GetEngagementMetricsAsync(metricsUsername);
                        var metricsJson = JsonSerializer.Serialize(metrics, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? GetEngagementMetricsAsync completed");
                        _logger.LogDebug("Result: {Result}", metricsJson);
                        return metricsJson;

                    case "get_publication_info":
                        _logger.LogInformation("?? Executing: GetPublicationInfoAsync");
                        var publicationId = GetStringParameter(parameters, "publication_id");
                        _logger.LogDebug("  Publication ID: {PublicationId}", publicationId);
                        
                        var publicationInfo = await _mcpServer.GetPublicationInfoAsync(publicationId);
                        var publicationInfoJson = JsonSerializer.Serialize(publicationInfo, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? GetPublicationInfoAsync completed");
                        _logger.LogDebug("Result: {Result}", publicationInfoJson);
                        return publicationInfoJson;

                    // ===== PHASE 1: CRITICAL FEATURES =====
                    
                    case "get_article_content":
                        _logger.LogInformation("?? Executing: GetArticleContentAsync");
                        var contentArticleId = GetStringParameter(parameters, "article_id");
                        var format = GetStringParameter(parameters, "format", defaultValue: "markdown");
                        _logger.LogDebug("  Article ID: {ArticleId}", contentArticleId);
                        _logger.LogDebug("  Format: {Format}", format);
                        
                        var articleContent = await _mcpServer.GetArticleContentAsync(contentArticleId, format);
                        var articleContentJson = JsonSerializer.Serialize(articleContent, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? GetArticleContentAsync completed");
                        _logger.LogDebug("Result: {Result}", articleContentJson);
                        return articleContentJson;

                    case "get_user_info_by_id":
                        _logger.LogInformation("?? Executing: GetUserInfoByIdAsync");
                        var userId = GetStringParameter(parameters, "user_id");
                        _logger.LogDebug("  User ID: {UserId}", userId);
                        
                        var userInfo = await _mcpServer.GetUserInfoByIdAsync(userId);
                        var userInfoJson = JsonSerializer.Serialize(userInfo, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? GetUserInfoByIdAsync completed");
                        _logger.LogDebug("Result: {Result}", userInfoJson);
                        return userInfoJson;

                    case "get_publication_articles":
                        _logger.LogInformation("?? Executing: GetPublicationArticlesAsync");
                        var pubSlugOrId = GetStringParameter(parameters, "publication_slug_or_id");
                        var pubLimit = GetIntParameter(parameters, "limit");
                        _logger.LogDebug("  Publication: {Publication}", pubSlugOrId);
                        _logger.LogDebug("  Limit: {Limit}", pubLimit?.ToString() ?? "none");
                        
                        var pubArticles = await _mcpServer.GetPublicationArticlesAsync(pubSlugOrId, pubLimit);
                        var pubArticlesJson = JsonSerializer.Serialize(pubArticles, new JsonSerializerOptions { WriteIndented = true });
                        
                        _logger.LogInformation("? GetPublicationArticlesAsync completed");
                        _logger.LogDebug("Result: {Result}", pubArticlesJson);
                        return pubArticlesJson;

                    default:
                        _logger.LogError("!!! Unknown tool: {ToolName} !!!", toolName);
                        return JsonSerializer.Serialize(new { error = $"Unknown tool: {toolName}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("????????????????????????????????????????");
                _logger.LogError("? TOOL EXECUTION ERROR");
                _logger.LogError("????????????????????????????????????????");
                _logger.LogError("Tool: {ToolName}", toolName);
                _logger.LogError("Exception Type: {ExceptionType}", ex.GetType().Name);
                _logger.LogError("Message: {Message}", ex.Message);
                _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
                
                return JsonSerializer.Serialize(new { 
                    error = ex.Message, 
                    stackTrace = ex.StackTrace,
                    toolName = toolName
                });
            }
        }

        private string GetStringParameter(Dictionary<string, object> parameters, string key)
        {
            _logger.LogDebug("GetStringParameter: {Key}", key);
            
            if (parameters.TryGetValue(key, out var value))
            {
                if (value is JsonElement jsonElement)
                {
                    var stringValue = jsonElement.GetString() ?? throw new ArgumentException($"Parameter '{key}' is required");
                    _logger.LogDebug("  ? Parsed as: {Value}", stringValue);
                    return stringValue;
                }
                var result = value?.ToString() ?? throw new ArgumentException($"Parameter '{key}' is required");
                _logger.LogDebug("  ? Parsed as: {Value}", result);
                return result;
            }
            
            _logger.LogError("  ? Parameter '{Key}' not found!", key);
            throw new ArgumentException($"Parameter '{key}' is required");
        }

        private string GetStringParameter(Dictionary<string, object> parameters, string key, string defaultValue)
        {
            _logger.LogDebug("GetStringParameter: {Key} (with default: {Default})", key, defaultValue);
            
            if (parameters.TryGetValue(key, out var value))
            {
                if (value is JsonElement jsonElement)
                {
                    var stringValue = jsonElement.GetString();
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        _logger.LogDebug("  ?? Parameter '{Key}' is empty, using default: {Default}", key, defaultValue);
                        return defaultValue;
                    }
                    _logger.LogDebug("  ? Parsed as: {Value}", stringValue);
                    return stringValue;
                }
                var result = value?.ToString();
                if (string.IsNullOrEmpty(result))
                {
                    _logger.LogDebug("  ?? Parameter '{Key}' is empty, using default: {Default}", key, defaultValue);
                    return defaultValue;
                }
                _logger.LogDebug("  ? Parsed as: {Value}", result);
                return result;
            }
            
            _logger.LogDebug("  ?? Parameter '{Key}' not found, using default: {Default}", key, defaultValue);
            return defaultValue;
        }

        private int? GetIntParameter(Dictionary<string, object> parameters, string key)
        {
            _logger.LogDebug("GetIntParameter: {Key}", key);
            
            if (parameters.TryGetValue(key, out var value))
            {
                if (value is JsonElement jsonElement)
                {
                    if (jsonElement.ValueKind == JsonValueKind.Number)
                    {
                        var numValue = jsonElement.GetInt32();
                        _logger.LogDebug("  ? Parsed as: {Value}", numValue);
                        return numValue;
                    }
                }
                if (value is int intValue)
                {
                    _logger.LogDebug("  ? Parsed as: {Value}", intValue);
                    return intValue;
                }
                if (int.TryParse(value?.ToString(), out var parsed))
                {
                    _logger.LogDebug("  ? Parsed as: {Value}", parsed);
                    return parsed;
                }
            }
            
            _logger.LogDebug("  ? Parameter '{Key}' not found or not a number, returning null", key);
            return null;
        }
    }

    /// <summary>
    /// Tool definition for MCP
    /// </summary>
    public class McpToolDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, McpToolParameter> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Tool parameter definition
    /// </summary>
    public class McpToolParameter
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Required { get; set; }
    }
}
