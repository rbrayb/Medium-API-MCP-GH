# Quick Start Guide - Medium MCP Server

## What is This?

This MCP (Model Context Protocol) server lets you query Medium blog statistics and content using natural language in GitHub Copilot, right inside Visual Studio or VS Code.

**Current Status:** 11 active tools | Phase 1 complete | Production ready ✅

## What You Can Do

- 📊 Get blog statistics (followers, articles, bio)
- 📝 Access full article content (markdown, HTML, or text)
- 🔍 Search articles and tags across Medium
- 📈 Analyze engagement metrics (claps, responses, voters)
- 📚 Browse publications and their articles
- 🏆 Find top-performing content
- 👤 Look up user information by username or ID

## Installation Steps

### 1. Prerequisites
- ✅ .NET 9.0 SDK installed
- ✅ GitHub Copilot subscription
- ✅ RapidAPI account with Medium2 API access

**Check .NET version:**
```bash
dotnet --version
# Should show 9.0.x or higher
```

### 2. Get Your API Key
1. Go to https://rapidapi.com
2. Subscribe to the Medium2 API: https://rapidapi.com/nishujain199719-vgIfuFHZxVZ/api/medium2
3. Copy your API key from the dashboard

### 3. Configure the Project
1. Open `appsettings.json` in your project directory
2. Replace the ApiKey with your actual key:
   ```json
   {
     "Medium": {
       "ApiKey": "YOUR-RAPIDAPI-KEY-HERE"
     }
   }
   ```
3. Save the file

### 4. Test the Application
Run in console mode first to verify everything works:

```bash
# Navigate to project directory
cd "path\to\Medium-API-MCP-GH"

# Build the project
dotnet build

# Run console mode
dotnet run
```

**Expected output:**
- You should see user statistics, article details, search results, and publication info
- No error messages about API keys or connectivity
- Data returned from Medium API

If you see errors, check:
- ✅ API key is correct in appsettings.json
- ✅ RapidAPI subscription is active
- ✅ Internet connection is working

### 5. Configure GitHub Copilot

#### For VS Code:

1. **Locate or create MCP configuration file:**
   - Windows: `%APPDATA%\Code\User\mcp.json`
   - macOS/Linux: `~/.config/Code/User/mcp.json`

2. **Add this configuration** (update the project path):
   ```json
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
   ```

3. **Restart VS Code completely**

#### For Visual Studio 2022:
1. The MCP configuration may vary based on your VS version
2. Check Visual Studio's GitHub Copilot extension settings
3. Look for MCP server configuration options

See [COPILOT-SETUP.md](COPILOT-SETUP.md) for detailed configuration instructions.

### 6. Verify Installation

Open GitHub Copilot Chat and test with:

```
"Get blog statistics for username jbloggs"
```

**If working correctly:**
- ✅ MCP server starts automatically
- ✅ Returns user statistics (followers, article count, bio)
- ✅ Response appears in 1-3 seconds

**If not working:**
- Check Output panel (View → Output → GitHub Copilot)
- Look for error messages
- Verify project path in mcp.json is correct

## Available Commands (11 Tools)

Here are the specific tools you can use:

### Original Tools (8)
| Tool | Purpose | Example Query |
|------|---------|---------------|
| **get_blog_statistics** | Get user's blog overview | "Get stats for jbloggs" |
| **get_user_articles** | List user's articles | "Show 5 articles for jbloggs" |
| **get_article_details** | Get specific article info | "Details for article abc123" |
| **search_articles** | Search articles by topic | "Search Azure articles" |
| **search_tags** | Find tags | "Find Entra tags" |
| **get_top_articles_by_claps** | Get top articles | "Top 10 articles by claps" |
| **get_engagement_metrics** | Get analytics | "Show engagement metrics" |
| **get_publication_info** | Get publication data | "Info for publication 123456" |

### Phase 1 Tools (3) ✅
| Tool | Purpose | Example Query |
|------|---------|---------------|
| **get_article_content** | Get full article content | "Get content for abc123 in markdown" |
| **get_user_info_by_id** | Fast user lookup by ID | "Get user info for ID 123456" |
| **get_publication_articles** | List publication articles | "Articles from 'towards-data-science'" |

## Usage Examples

### Basic Queries
```
"Get blog statistics for jbloggs"
"How many followers does jbloggs have?"
"Show me article details for xyz123"
```

### Content Access (Phase 1 ✅)
```
"Get article content for abc123 in markdown"
"Show me the full text of article xyz789"
"Get article content as HTML for abc123"
```

### Article Analysis
```
"What are my top 10 articles by claps?"
"Show me my last 5 articles"
"Get engagement metrics for jbloggs"
```

### Search & Discovery
```
"Search for articles about Azure AD"
"Find articles about Verifiable Credentials"
"Search for tags related to Custom Policies"
```

### Publication Management (Phase 1 ✅)
```
"Get information about publication ID 123456"
"Show me articles from publication 'medium'"
"List 10 articles from publication abc123"
```

### User Lookup
```
"Get user info for ID 123456"
"Look up user jbloggs by username"
```

## Testing Without Copilot

You can test the MCP server directly using the provided test script:

### Option 1: PowerShell Script
```powershell
.\test-mcp.ps1
```

### Option 2: Manual Testing
```bash
# Start server
dotnet run -- --mcp

# You should see startup messages
# Server is now listening for JSON-RPC commands via stdin
```

### Option 3: MCP Inspector (Recommended)
```bash
# Install MCP Inspector
npm install -g @modelcontextprotocol/inspector

# Run with your server
npx @modelcontextprotocol/inspector dotnet run -- --mcp
```

This opens a web UI where you can test all 11 tools interactively.

See [TESTING-GUIDE.md](TESTING-GUIDE.md) for comprehensive testing instructions.

## Troubleshooting

### "Server not responding"
**Problem:** GitHub Copilot can't connect to the MCP server

**Solutions:**
1. Check project path in mcp.json is correct and uses absolute path
2. Verify .NET 9.0 is installed: `dotnet --version`
3. Test manually: `dotnet run -- --mcp` (should not error)
4. Restart VS Code completely
5. Check Output panel for error messages

### "API key error" or "401 Unauthorized"
**Problem:** RapidAPI key is invalid or missing

**Solutions:**
1. Verify appsettings.json has your correct RapidAPI key
2. Check the key on RapidAPI dashboard (https://rapidapi.com)
3. Ensure Medium2 API subscription is active
4. Confirm you haven't exceeded API rate limits
5. Try the key in console mode first: `dotnet run`

### "No results found"
**Problem:** Query returns empty results

**Solutions:**
1. Verify the Medium username exists and is spelled correctly
2. Try with a known username (e.g., "jbloggs")
3. Check that the user has public articles
4. For article IDs, ensure they are valid Medium article IDs

### "Build failed" or "Cannot find dotnet"
**Problem:** .NET SDK not found or project won't build

**Solutions:**
1. Install .NET 9.0 SDK from https://dotnet.microsoft.com/download
2. Restart your terminal/IDE after installation
3. Run `dotnet --version` to verify installation
4. Run `dotnet restore` to restore NuGet packages
5. Run `dotnet build` to check for build errors

### Server starts but times out
**Problem:** Server starts but requests timeout

**Solutions:**
1. Check internet connectivity
2. Verify RapidAPI service is operational
3. Check firewall isn't blocking dotnet.exe
4. Look for rate limit messages in logs
5. Enable debug logging to see detailed errors

## Performance Tips

1. **Use limit parameters** for list operations:
   ```
   "Show me 5 articles" (fast)
   vs
   "Show me all articles" (slow)
   ```

2. **Cache frequently accessed data** when possible

3. **Use user ID lookups** when you have the ID:
   ```
   "Get user info for ID 123" (faster)
   vs
   "Get user info for username john" (slower)
   ```

4. **Check API quota** on RapidAPI dashboard regularly

5. **Monitor response times** - typical: 1-5 seconds

## Next Steps

Once everything is working:

1. ✅ **Explore all 11 tools** - Try different queries
2. ✅ **Customize tools** - Edit McpToolHandler.cs to add features
3. ✅ **Add new tools** - Extend McpServer.cs with new methods
4. ✅ **Share with team** - Distribute your MCP configuration
5. ✅ **Monitor usage** - Check RapidAPI dashboard for stats

## Advanced Configuration

### Enable Debug Logging
```json
{
  "env": {
    "Logging__LogLevel__Default": "Debug"
  }
}
```

### Custom Project Location
Update the project path in mcp.json to your installation directory.

### Multiple MCP Servers
You can configure multiple MCP servers in the same mcp.json file.

See [COPILOT-SETUP.md](COPILOT-SETUP.md) for advanced configuration options.

## Documentation

- **[README.md](README.md)** - Project overview
- **[README-MCP.md](README-MCP.md)** - Detailed tool reference
- **[COPILOT-SETUP.md](COPILOT-SETUP.md)** - Configuration guide
- **[TESTING-GUIDE.md](TESTING-GUIDE.md)** - Testing instructions
- **[API-QUICK-REFERENCE.md](API-QUICK-REFERENCE.md)** - API coverage
- **[DEBUG-LOGGING-QUICK-REFERENCE.md](DEBUG-LOGGING-QUICK-REFERENCE.md)** - Debugging

## Support Resources

- **Medium API**: https://rapidapi.com/nishujain199719-vgIfuFHZxVZ/api/medium2
- **MCP Protocol**: https://modelcontextprotocol.io/
- **GitHub Copilot**: https://github.com/features/copilot
- **Project Issues**: https://github.com/rbrayb/Medium-API-MCP-GH/issues

## Quick Reference Card

### Essential Commands
```bash
# Build project
dotnet build

# Run console mode (testing)
dotnet run

# Run MCP server mode
dotnet run -- --mcp

# Check .NET version
dotnet --version

# Restore packages
dotnet restore
```

### Configuration Files
- `appsettings.json` - API key configuration
- `mcp-config.json` - Alternative MCP configuration
- VS Code: `%APPDATA%\Code\User\mcp.json` - Main MCP configuration

### Status Indicators
- ✅ Working correctly
- ⚠️ Warning or needs attention
- ❌ Error or not working
- 🔄 In progress
- 📋 Planned for future

---

**Happy blogging with AI! 🎉**

**Current Version:** Phase 1 Complete  
**Total Tools:** 11 (8 original + 3 Phase 1)  
**API Coverage:** 61% (11/18 methods)  
**Status:** Production Ready ✅
