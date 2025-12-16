# Medium Blog Statistics MCP Server

This project provides a Model Context Protocol (MCP) server that integrates with GitHub Copilot to give you natural language access to Medium blog statistics and content.

## Overview

The MCP server allows you to query Medium blog data using natural language in GitHub Copilot. Built with .NET 9 and the unofficial Medium API, it provides powerful analytics, content access, and discovery features.

**Current Status:** 11 active tools | Phase 1 complete | Production ready ✅

## What You Can Do

Ask questions like:
- "What are my blog statistics for username jbloggs?"
- "Show me my top 10 articles by claps"
- "Get the full content of article ID abc123 in markdown"
- "Search for articles about Azure Active Directory"
- "What are the engagement metrics for my blog?"
- "List articles from publication 'towards-data-science'"

## Architecture

The project consists of:
- **McpServer**: Core server implementing 11 tools for Medium API access
- **McpProtocolHandler**: Implements the MCP protocol for stdio communication
- **McpToolHandler**: Routes tool calls to appropriate server methods
- **McpModels**: Data models for structured responses
- **McpServiceExtensions**: Dependency injection configuration

## Available Tools (11 Total)

### Original Tools (Phase 0) - 8 tools

#### 1. `get_blog_statistics`
Get comprehensive blog statistics including follower count, article count, bio, and social links.

**Parameters:**
- `username` (required): Medium username (e.g., 'jbloggs')

**Example:** "What are the statistics for user jbloggs?"

**Returns:** User ID, full name, followers count, article count, bio, profile image, Twitter username

---

#### 2. `get_article_details`
Get detailed information about a specific article including claps, responses, voters, tags, and topics.

**Parameters:**
- `article_id` (required): Medium article ID

**Example:** "Get details for article xyz123"

**Returns:** Article metadata, engagement stats (claps, responses, voters), URL, published date, tags, topics

---

#### 3. `get_user_articles`
Get all articles for a Medium user with optional limit for performance.

**Parameters:**
- `username` (required): Medium username
- `limit` (optional): Maximum number of articles to return

**Example:** "Show me the last 5 articles for jbloggs"

**Returns:** List of articles with full details (title, claps, responses, URL, etc.)

---

#### 4. `search_articles`
Search for Medium articles by query string with optional result limit.

**Parameters:**
- `query` (required): Search query
- `limit` (optional): Maximum number of results to return

**Example:** "Search for articles about Verifiable Credentials"

**Returns:** Matching articles with full details and engagement metrics

---

#### 5. `search_tags`
Search for Medium tags and get information about article and author counts.

**Parameters:**
- `query` (required): Tag search query

**Example:** "Find tags related to Entra External ID"

**Returns:** List of tags with article counts and author counts

---

#### 6. `get_top_articles_by_claps`
Get the top performing articles for a user ranked by number of claps.

**Parameters:**
- `username` (required): Medium username
- `top_count` (optional): Number of top articles (default: 10)

**Example:** "Show me my top 10 articles by claps"

**Returns:** Ranked list of articles by clap count with full details

---

#### 7. `get_engagement_metrics`
Get comprehensive engagement metrics including total and average claps, responses, and voters.

**Parameters:**
- `username` (required): Medium username

**Example:** "What are my engagement metrics?"

**Returns:** Total articles, total claps/responses/voters, averages per article

---

#### 8. `get_publication_info`
Get publication information including name, tagline, description, followers, tags, and social media links.

**Parameters:**
- `publication_id` (required): Medium publication ID

**Example:** "Get information about publication ID 123456"

**Returns:** Publication metadata, follower count, social links, editors, tags

---

### Phase 1 Tools (Critical Features) - 3 tools ✅

#### 9. `get_article_content`
Get full article content in markdown, HTML, or plain text format. Enables content analysis, archiving, and full-text processing.

**Parameters:**
- `article_id` (required): Medium article ID
- `format` (optional): Content format - 'markdown' (default), 'html', or 'text'

**Example:** "Get article content for abc123 in markdown format"

**Returns:** Full article content, title, subtitle, URL, published date, content length

**Use Cases:**
- Content archiving and backup
- Text analysis and NLP processing
- Content migration to other platforms
- Offline reading
- Custom formatting and styling

---

#### 10. `get_user_info_by_id`
Get user information by user ID. Faster than username lookup when you already have the user ID.

**Parameters:**
- `user_id` (required): Medium user ID

**Example:** "Get user info for ID 123456"

**Returns:** User ID, username, full name, followers, bio, profile image, Twitter username, profile URL

**Performance:** Faster than username lookup - direct ID-based query

---

#### 11. `get_publication_articles`
Get all articles published in a specific Medium publication. Accepts either publication slug (name) or publication ID.

**Parameters:**
- `publication_slug_or_id` (required): Publication slug (e.g., 'towards-data-science') or publication ID
- `limit` (optional): Maximum number of articles to return

**Example:** "Show me articles from publication 'towards-data-science'"

**Returns:** Publication metadata, article list with full details, total article count

**Note:** Can search by friendly slug name or numeric ID

---

## Setup

### Prerequisites
- .NET 9.0 SDK
- GitHub Copilot subscription
- RapidAPI key for Medium2 API

### Configuration

1. **Update appsettings.json** with your RapidAPI key:
```json
{
  "Medium": {
    "ApiKey": "your-rapidapi-key-here"
  }
}
```

2. **Configure GitHub Copilot** - See [COPILOT-SETUP.md](COPILOT-SETUP.md) for detailed instructions.

Example VS Code configuration:
"medium-stats-gh": {
            "type": "stdio",
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "x:\\...\\Medium API MCP GH\\Medium-API-MCP-GH\\Medium API MCP GH.csproj"                
            ],
            "env": {
                "MCP_MODE": "true",
                "ApiKey": "api-key",
                "Logging__LogLevel__Default": "Information"
            }
        }
  }
}
```

Or use the provided `mcp-config.json` file (update the path).

### Running the Application

**As Console Application (testing):**
```bash
dotnet run
```
Runs demo queries to verify API connectivity.

**As MCP Server (production):**
```bash
dotnet run -- --mcp
```
Starts as MCP server for GitHub Copilot integration.

**With Environment Variable:**
```bash
set MCP_MODE=true
dotnet run
```

## Usage Examples

Once configured with GitHub Copilot, you can ask natural language questions:

### Basic Statistics
```
"Get blog statistics for jbloggs"
"How many followers does jbloggs have?"
"What's the article count for my blog?"
```

### Article Content Access (Phase 1 ✅)
```
"Get the full content of article abc123 in markdown"
"Show me article xyz789 as HTML"
"Get article content as plain text for abc123"
```

### User Lookup
```
"Get user information for ID 123456"
"Look up user by username jbloggs"
```

### Article Queries
```
"Show me my latest 10 articles"
"Get details for article abc123"
"What are my top 5 articles by claps?"
```

### Search & Discovery
```
"Search for articles about Azure AD"
"Find articles related to Verifiable Credentials"
"Search for tags about Custom Policies"
```

### Publication Management (Phase 1 ✅)
```
"Get publication information for ID 123456"
"Show me articles from publication 'towards-data-science'"
"List articles in publication ID abc123"
```

### Analytics
```
"What are my engagement metrics?"
"Show me my best performing articles"
"What's the average number of claps per article?"
```

## Project Structure

```
Medium-API-MCP-GH/
├── MCP/
│   ├── McpServer.cs              # Core server with 11 tools
│   ├── McpProtocolHandler.cs     # MCP protocol implementation
│   ├── McpToolHandler.cs         # Tool routing and definitions (11 tools)
│   ├── McpModels.cs              # Response data models
│   └── McpServiceExtensions.cs   # DI registration
├── Program.cs                     # Entry point (console & MCP modes)
├── appsettings.json              # Configuration (API key)
├── mcp-config.json               # MCP server configuration
└── Medium API MCP GH.csproj      # Project file (.NET 9)
```

## API Coverage

| Category | Coverage | Status |
|----------|----------|--------|
| Search API | 100% (2/2) | ✅ Complete |
| Platform API | 100% (1/1) | ✅ Complete |
| Publications API | 75% (3/4) | ⚠️ Mostly Complete |
| Articles API | 44% (4/9) | 🔄 In Progress |
| Users API | 29% (2/7) | 🔄 In Progress |
| **Overall** | **61% (11/18)** | **Phase 1 Complete** |

See [API-QUICK-REFERENCE.md](API-QUICK-REFERENCE.md) for detailed API coverage.

## API Source

This project uses the Medium2 API from RapidAPI:
- API URL: https://rapidapi.com/nishujain199719-vgIfuFHZxVZ/api/medium2
- Medium SDK: https://github.com/martinstm/medium-dotnet-sdk
- SDK Version: Medium.Client 1.1.0

## Troubleshooting

### MCP Server Not Responding
1. Verify the project path in mcp-config.json matches your installation
2. Check that appsettings.json has the correct API key
3. Ensure .NET 9.0 SDK is installed: `dotnet --version`
4. Test manually: `dotnet run -- --mcp`
5. Check logs in the Output window (View → Output → GitHub Copilot)

### API Errors (401 Unauthorized)
1. Verify your RapidAPI key is valid and active
2. Check API rate limits on RapidAPI dashboard
3. Ensure the Medium2 API subscription is active
4. Confirm the key has access to Medium2 API

### Tool Not Found
1. Restart GitHub Copilot completely
2. Verify mcp-config.json syntax is correct (use a JSON validator)
3. Check that the MCP server is running (test with `dotnet run -- --mcp`)
4. Review server logs for initialization errors

### Slow Performance
1. Use `limit` parameters for list operations (e.g., `limit: 5`)
2. Cache frequently accessed data if possible
3. Check network connectivity to RapidAPI
4. Monitor API quota usage on RapidAPI dashboard

See [TESTING-GUIDE.md](TESTING-GUIDE.md) for comprehensive troubleshooting.

## Development

### Adding New Tools

To add new tools:

1. **Add method to McpServer.cs**
   ```csharp
   public async Task<YourResult> YourNewMethodAsync(string param)
   {
       // Implementation
   }
   ```

2. **Create response model in McpModels.cs**
   ```csharp
   public class YourResult
   {
       public bool Success { get; set; }
       // ... other properties
   }
   ```

3. **Add tool definition in McpToolHandler.GetToolDefinitions()**
   ```csharp
   new McpToolDefinition
   {
       Name = "your_new_tool",
       Description = "Description of what it does",
       Parameters = new()
       {
           { "param", new McpToolParameter { Type = "string", Description = "...", Required = true }}
       }
   }
   ```

4. **Add routing logic in McpToolHandler.HandleToolCallAsync()**
   ```csharp
   case "your_new_tool":
       var result = await _mcpServer.YourNewMethodAsync(param);
       return JsonSerializer.Serialize(result);
   ```

### Testing

```bash
# Build the project
dotnet build

# Run console mode tests
dotnet run

# Run as MCP server
dotnet run -- --mcp

# Run with debug logging
set Logging__LogLevel__Default=Debug
dotnet run -- --mcp
```

See [TESTING-GUIDE.md](TESTING-GUIDE.md) for detailed testing instructions including MCP Inspector usage.

## Documentation

- **[README.md](README.md)** - Project overview and features
- **[QUICKSTART.md](QUICKSTART.md)** - Step-by-step setup guide
- **[COPILOT-SETUP.md](COPILOT-SETUP.md)** - GitHub Copilot configuration
- **[TESTING-GUIDE.md](TESTING-GUIDE.md)** - Complete testing instructions
- **[API-QUICK-REFERENCE.md](API-QUICK-REFERENCE.md)** - API coverage and roadmap
- **[DEBUG-LOGGING-QUICK-REFERENCE.md](DEBUG-LOGGING-QUICK-REFERENCE.md)** - Debugging tips
- **[README-MCP.md](README-MCP.md)** - This file (detailed tool reference)

## Roadmap

### Phase 1 (Current) ✅ COMPLETE
- ✅ 8 original tools (blog statistics, articles, search, analytics)
- ✅ 3 critical tools (article content, user ID lookup, publication articles)
- ✅ Total: 11 active tools

## Performance Notes

| Tool | Typical Response Time |
|------|----------------------|
| get_blog_statistics | 1-2 seconds |
| get_article_content | 2-3 seconds |
| get_user_articles (limit=5) | 2-4 seconds |
| get_user_articles (all) | 3-10 seconds |
| search_articles | 2-4 seconds |
| get_publication_articles | 3-8 seconds |

**Tip:** Always use `limit` parameters when dealing with large datasets to improve response times.

## License

This project uses the Medium.Client NuGet package which has its own license terms.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests for improvements.

## Support

For questions about:
- **Medium API**: https://rapidapi.com/nishujain199719-vgIfuFHZxVZ/api/medium2
- **MCP Protocol**: https://modelcontextprotocol.io/
- **GitHub Copilot**: https://github.com/features/copilot
- **Project Issues**: https://github.com/rbrayb/Medium-API-MCP-GH/issues

---

**Current Version:** Phase 1 Complete  
**Total Tools:** 11 (8 original + 3 Phase 1)  
**API Coverage:** 61% (11/18 methods)  
**Status:** Production Ready ✅
