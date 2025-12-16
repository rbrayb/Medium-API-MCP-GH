# GitHub Copilot MCP Configuration

## Overview

This document explains how to configure GitHub Copilot to use the Medium MCP Server, enabling natural language queries for Medium blog statistics and content.

## Configuration for VS Code

### Step 1: Locate MCP Configuration File

The MCP configuration file is typically located at:

**Windows:**
```
%APPDATA%\Code\User\mcp.json
```
or
```
C:\Users\[YourUsername]\AppData\Roaming\Code\User\mcp.json
```

**macOS/Linux:**
```
~/.config/Code/User/mcp.json
```

### Step 2: Add Server Configuration

Add the following configuration to your `mcp.json` file:

```jsonc
{
  "github.copilot.advanced": {
    "mcp": {
      "servers": {
        "medium-stats-gh": {
          "command": "dotnet",
          "args": [
            "run",
            "--project",
            "x:\\...\\Medium API MCP GH\\Medium-API-MCP-GH\\Medium API MCP GH.csproj",
            "--",
            "--mcp"
          ],
          "description": "Medium blog statistics and content via MCP"
        }
      }
    }
  }
}
```

**Important:** Update the project path to match your local installation directory.

### Step 3: Restart VS Code

After updating the configuration:
1. Save the `mcp.json` file
2. Close and reopen VS Code
3. GitHub Copilot will automatically start the MCP server when needed

## Configuration for Visual Studio 2022

GitHub Copilot's MCP support in Visual Studio may vary by version. Check the GitHub Copilot extension settings for MCP server configuration options.

## Alternative Configuration Methods

### Using Environment Variables

You can also configure the server using environment variables in the MCP configuration:

```jsonc
{
  "github.copilot.advanced": {
    "mcp": {
      "servers": {
        "medium-stats-gh": {
          "command": "dotnet",
          "args": [
            "run",
            "--project",
            "x:\\...\\Medium API MCP GH\\Medium-API-MCP-GH\\Medium API MCP GH.csproj",
            "--",
            "--mcp"
          ],
          "env": {
            "ApiKey": "your-rapidapi-key-here",
            "Logging__LogLevel__Default": "Information"
          },
          "description": "Medium blog statistics and content via MCP"
        }
      }
    }
  }
}
```

**Note:** If you provide the API key in the MCP configuration, it will override the value in `appsettings.json`.

## Available Tools

Once configured, you can use these 11 tools through natural language in GitHub Copilot:

### Original Tools (Phase 0)
1. **get_blog_statistics** - User statistics and overview
2. **get_article_details** - Detailed article information
3. **get_user_articles** - List user's articles
4. **search_articles** - Search Medium articles
5. **search_tags** - Find and analyze tags
6. **get_top_articles_by_claps** - Best performing articles
7. **get_engagement_metrics** - Analytics and metrics
8. **get_publication_info** - Publication metadata

### Phase 1 Tools (Critical Features)
9. **get_article_content** - Full article content (markdown/HTML/text)
10. **get_user_info_by_id** - User lookup by ID
11. **get_publication_articles** - Publication's articles

## Usage Examples

### Basic Queries
```
"Get blog statistics for username jbloggs"
"How many followers does jbloggs have?"
"Show me article details for ID abc123"
```

### Content Access
```
"Get article content for ID abc123 in markdown format"
"Show me the full text of article xyz789"
"Get article content as HTML for abc123"
```

### Analytics
```
"What are my top 10 articles by claps?"
"Calculate engagement metrics for jbloggs"
"Show me the most popular articles"
```

### Search & Discovery
```
"Search for articles about Azure Active Directory"
"Find articles related to Verifiable Credentials"
"Search for tags about Custom Policies"
```

### Publications
```
"Get information about publication ID 123456"
"Show me articles from publication 'towards-data-science'"
"List articles in publication ID abc123"
```

## Verifying Installation

### Method 1: Test with GitHub Copilot

Open Copilot Chat in VS Code and type:
```
"Get blog statistics for username jbloggs"
```

If configured correctly, Copilot will:
1. Start the MCP server
2. Call the appropriate tool
3. Return formatted results

### Method 2: Check Server Logs

Enable debug logging to see server activity:

1. Update your MCP configuration to include logging:
```jsonc
{
  "env": {
    "Logging__LogLevel__Default": "Debug"
  }
}
```

2. Open VS Code Output panel (View ? Output)
3. Select "GitHub Copilot" from the dropdown
4. Look for MCP server startup messages

### Method 3: Manual Test

Test the server manually:
```bash
cd "x:\...\Medium API MCP GH\Medium-API-MCP-GH"
dotnet run -- --mcp
```

You should see:
```
========================================
Medium MCP Server started. Listening for requests...
Protocol Version: 2024-11-05
Server Name: medium-stats
Server Version: 1.0.0
========================================
```

## Troubleshooting

### "Server not responding"

**Cause:** Project path is incorrect or .NET SDK not found

**Solution:**
1. Verify the project path in `mcp.json` matches your installation
2. Ensure .NET 9.0 SDK is installed: `dotnet --version`
3. Test manually: `dotnet run -- --mcp`

### "API key error"

**Cause:** RapidAPI key not configured or invalid

**Solution:**
1. Check `appsettings.json` has the correct API key
2. Verify the API key on RapidAPI dashboard
3. Ensure Medium2 API subscription is active

### "Tool not found"

**Cause:** Server not started or configuration issue

**Solution:**
1. Restart VS Code completely
2. Check MCP configuration syntax is correct
3. Verify server starts manually
4. Check GitHub Copilot extension is up to date

### Server starts but no response

**Cause:** Communication issue or server crash

**Solution:**
1. Enable debug logging to see errors
2. Check Output panel for error messages
3. Review server logs for exceptions
4. Test server manually with test script

## Advanced Configuration

### Custom Working Directory

```jsonc
{
  "command": "dotnet",
  "args": ["run", "--project", "path/to/project.csproj", "--", "--mcp"],
  "cwd": "x:\\...\\Medium API MCP GH\\Medium-API-MCP-GH"
}
```

### Debug Mode

```jsonc
{
  "env": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "Logging__LogLevel__Default": "Debug",
    "Logging__LogLevel__Medium": "Debug"
  }
}
```

### Multiple Servers

You can configure multiple MCP servers:

```jsonc
{
  "github.copilot.advanced": {
    "mcp": {
      "servers": {
        "medium-stats-gh": {
          "command": "dotnet",
          "args": [...],
          "description": "Medium blog statistics"
        },
        "other-server": {
          "command": "python",
          "args": [...],
          "description": "Other MCP server"
        }
      }
    }
  }
}
```

## Additional Resources

- **Testing Guide**: [TESTING-GUIDE.md](TESTING-GUIDE.md)
- **Quick Start**: [QUICKSTART.md](QUICKSTART.md)
- **Debug Reference**: [DEBUG-LOGGING-QUICK-REFERENCE.md](DEBUG-LOGGING-QUICK-REFERENCE.md)
- **MCP Protocol**: https://modelcontextprotocol.io/
- **GitHub Copilot Docs**: https://github.com/features/copilot

## Getting Help

If you encounter issues:

1. **Check the logs** in VS Code Output panel
2. **Test manually** using `dotnet run -- --mcp`
3. **Verify configuration** paths and API keys
4. **Review documentation** for examples
5. **Create an issue** on GitHub with full error details

---

**Configuration Status**: 11 tools available | Phase 1 complete | Production ready

