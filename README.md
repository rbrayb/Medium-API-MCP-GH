# Medium-API-MCP-GH

A Model Context Protocol (MCP) server that provides natural language access to Medium blog statistics and content through GitHub Copilot. Query Medium articles, user statistics, publications, and more using conversational AI.

## Overview

This MCP server integrates with GitHub Copilot to enable natural language queries for Medium blog data. Built with .NET 9 and the unofficial Medium API, it provides 11 powerful tools for analyzing blog performance, content discovery, and audience engagement.

## Features

### ?? Blog Analytics
- Get comprehensive user statistics (followers, articles, bio, social links)
- Calculate engagement metrics (claps, responses, voters)
- Identify top-performing articles by claps
- Track article counts and publication history

### ?? Content Access
- Retrieve full article content in markdown, HTML, or plain text
- Get detailed article metadata (title, subtitle, tags, topics)
- Access article URLs, publication dates, and engagement data

### ?? Search & Discovery
- Search articles by keyword or topic
- Find related tags with article/author counts
- Discover articles across Medium's platform

### ?? Publication Management
- Get publication information and statistics
- List all articles in a publication
- Access publication metadata and social links

### ?? User Information
- Look up users by username or user ID
- Get follower counts and user profiles
- Access user bios and social media links

## Available Tools

### Original Tools (Phase 0)
1. **get_blog_statistics** - Comprehensive user blog overview
2. **get_article_details** - Detailed article information
3. **get_user_articles** - List user's articles with optional limit
4. **search_articles** - Search articles by query
5. **search_tags** - Find tags and get metadata
6. **get_top_articles_by_claps** - Get best performing articles
7. **get_engagement_metrics** - Calculate comprehensive analytics
8. **get_publication_info** - Get publication metadata

### Phase 1 Tools (Critical Features)
9. **get_article_content** - Get full article content (markdown/HTML/text)
10. **get_user_info_by_id** - Fast user lookup by ID
11. **get_publication_articles** - List publication's articles

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- GitHub Copilot subscription
- RapidAPI key for Medium2 API

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/rbrayb/Medium-API-MCP-GH.git
cd Medium-API-MCP-GH
```

2. **Configure API key**

Update `appsettings.json`:
```json
{
  "Medium": {
    "ApiKey": "your-rapidapi-key-here"
  }
}
```

3. **Test the application**
```bash
dotnet run
```

4. **Configure GitHub Copilot**

See [COPILOT-SETUP.md](COPILOT-SETUP.md) for detailed setup instructions.

## Usage Examples

Once configured with GitHub Copilot, ask natural language questions:

```
"Get blog statistics for username jbloggs"
"Show me the top 10 articles by claps for jbloggs"
"Get article content for ID abc123 in markdown format"
"Search for articles about Azure Active Directory"
"What are the engagement metrics for my blog?"
"Get publication info for ID 123456"
```

## Documentation

- **[QUICKSTART.md](QUICKSTART.md)** - Step-by-step setup guide
- **[COPILOT-SETUP.md](COPILOT-SETUP.md)** - GitHub Copilot configuration
- **[TESTING-GUIDE.md](TESTING-GUIDE.md)** - Complete testing instructions
- **[API-QUICK-REFERENCE.md](API-QUICK-REFERENCE.md)** - API coverage and roadmap
- **[DEBUG-LOGGING-QUICK-REFERENCE.md](DEBUG-LOGGING-QUICK-REFERENCE.md)** - Debugging tips

## Project Structure

```
Medium-API-MCP-GH/
??? MCP/
?   ??? McpServer.cs              # Core server with 11 tools
?   ??? McpProtocolHandler.cs     # MCP protocol implementation
?   ??? McpToolHandler.cs         # Tool routing and definitions
?   ??? McpModels.cs              # Response data models
?   ??? McpServiceExtensions.cs   # Dependency injection
??? Program.cs                     # Entry point (console & MCP modes)
??? appsettings.json              # Configuration
??? mcp-config.json               # MCP server configuration
??? Medium API MCP GH.csproj      # Project file
```

## Technology Stack

- **.NET 9.0** - Target framework
- **Medium.Client 1.1.0** - Medium API SDK
- **Microsoft.Extensions.Hosting** - Dependency injection and hosting
- **Model Context Protocol (MCP)** - GitHub Copilot integration

## API Source

This project uses the Medium2 API from RapidAPI:
- **API URL**: https://rapidapi.com/nishujain199719-vgIfuFHZxVZ/api/medium2
- **SDK**: https://github.com/martinstm/medium-dotnet-sdk

## Running Modes

### Console Mode (Testing)
```bash
dotnet run
```
Runs demo queries to verify API connectivity.

### MCP Server Mode (Production)
```bash
dotnet run -- --mcp
```
Starts as MCP server for GitHub Copilot integration.

## Troubleshooting

### Common Issues

**API Key Errors**
- Verify API key in `appsettings.json`
- Check RapidAPI subscription is active
- Ensure key has Medium2 API access

**Server Not Responding**
- Verify project path in MCP configuration
- Check .NET 9.0 SDK is installed
- Review logs in Output window

**Tool Not Found**
- Restart GitHub Copilot
- Verify MCP configuration is correct
- Check server is running in MCP mode

See [TESTING-GUIDE.md](TESTING-GUIDE.md) for detailed troubleshooting.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project uses the Medium.Client NuGet package which has its own license terms.

## Support

- **Medium API**: https://rapidapi.com/nishujain199719-vgIfuFHZxVZ/api/medium2
- **MCP Protocol**: https://modelcontextprotocol.io/
- **GitHub Issues**: https://github.com/rbrayb/Medium-API-MCP-GH/issues

## Acknowledgments

- Medium.Client SDK by Martin Stamenov
- Medium2 API on RapidAPI
- Model Context Protocol by Anthropic

---

**Current Status**: 11 active tools | Phase 1 complete | Production ready
