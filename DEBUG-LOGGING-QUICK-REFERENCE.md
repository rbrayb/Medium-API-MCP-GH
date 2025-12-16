# Debug Logging Quick Reference

## Quick Enable Debug Logging

### Option 1: Environment Variable (Fastest)
```bash
set Logging__LogLevel__Default=Debug
dotnet run
```

### Option 2: appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

---

## Log Symbol Cheat Sheet

| Symbol | Meaning | When You See It |
|--------|---------|-----------------|
| `>>>` | Incoming Request | Client sent something |
| `<<<` | Outgoing Response | Server responding |
| `??` | Action Starting | About to do something |
| `?` | Success | Operation succeeded |
| `?` | Failure | Operation failed |
| `??` | Warning | Non-critical issue |
| `!!!` | Error/Critical | Something bad happened |
| `??` | Method Entry | Function called |
| `===` | Major Section | Big separator |
| `---` | Subsection | Small separator |

---

## Common Log Patterns

### ? Everything Working
```
========================================
Medium MCP Server started...
========================================
>>> INCOMING REQUEST: Method='initialize', ID='1'
? Initialization complete
>>> INCOMING REQUEST: Method='tools/list', ID='2'
Discovered 11 tools
? Tool discovery complete
>>> INCOMING REQUEST: Method='tools/call', ID='3'
========================================
?? TOOL EXECUTION: get_blog_statistics
========================================
?? GetBlogStatisticsAsync - Username: jbloggs
  ?? Calling Medium API: Users.GetInfoByUsernameAsync
  ? User info retrieved
? Tool execution completed in XXXms
```

### ? Missing API Key
```
?? GetBlogStatisticsAsync - Username: jbloggs
  ?? Calling Medium API: Users.GetInfoByUsernameAsync
? Error in GetBlogStatisticsAsync
Exception Type: MissingConfigurationException
Message: The Apikey variable is not defined
```

### ? Invalid Parameters
```
>>> INCOMING REQUEST: Method='tools/call', ID='4'
!!! Missing 'name' parameter in tool call !!!
!!! SENDING ERROR RESPONSE !!!
Error Code: invalid_params
```

### ? API Error (401)
```
?? GetBlogStatisticsAsync - Username: jbloggs
  ?? Calling Medium API: Users.GetInfoByUsernameAsync
? Error in GetBlogStatisticsAsync
Exception Type: HttpRequestException
Message: 401 (Unauthorized)
```

### ? JSON Parse Error
```
========================================
RAW REQUEST RECEIVED:
{invalid json...
========================================
!!! JSON PARSE ERROR !!!
```

---

## Troubleshooting Checklist

### Server Not Starting?
Look for:
```
========================================
Medium MCP Server started...
========================================
```
If missing ? Check project path in VS Code settings.json

### Tools Not Showing?
Look for:
```
>>> INCOMING REQUEST: Method='tools/list'
Discovered 11 tools:
  ?? get_blog_statistics
  ?? get_article_details
  ?? get_user_articles
  ?? search_articles
  ?? search_tags
  ?? get_top_articles_by_claps
  ?? get_engagement_metrics
  ?? get_publication_info
  ?? get_article_content (Phase 1)
  ?? get_user_info_by_id (Phase 1)
  ?? get_publication_articles (Phase 1)
```
If missing ? Check MCP server configuration
If tool count is not 11 ? Check McpToolHandler

### Tool Execution Failing?
Look for:
```
========================================
?? TOOL EXECUTION: tool_name
========================================
```
Then check for:
- Parameter parsing errors
- API call errors
- Exception messages

### API Calls Failing?
Look for:
```
?? Calling Medium API: ...
```
Followed by:
- `?` = Success
- `?` = Failure (check exception)

---

## Where to Look for Logs

### VS Code
1. View ? Output (Ctrl+Shift+U)
2. Select "GitHub Copilot" from dropdown
3. Scroll to see MCP server logs

### Console (Manual Test)
```bash
cd "x:\...\Medium API MCP GH\Medium-API-MCP-GH"
set MCP_MODE=true
set ApiKey=your-key-here
set Logging__LogLevel__Default=Debug
dotnet run
```

### PowerShell Test Script
```powershell
# test-mcp.ps1
$env:MCP_MODE = "true"
$env:ApiKey = "your-key-here"
$env:Logging__LogLevel__Default = "Debug"
dotnet run
```

---

## Most Useful Log Filters

### See Only Errors
```
Logging__LogLevel__Default=Error
```

### See Operations But Not Details
```
Logging__LogLevel__Default=Information
```

### See Everything
```
Logging__LogLevel__Default=Debug
```

### See MCP Details Only
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Medium.Demos.ConsoleApp.MCP": "Debug"
    }
  }
}
```

---

## Request/Response Inspection

### Raw Request
```
========================================
RAW REQUEST RECEIVED:
{"jsonrpc":"2.0","id":1,"method":"..."}
========================================
```

### Raw Response
```
----------------------------------------
RAW RESPONSE SENT:
{"jsonrpc":"2.0","id":1,"result":{...}}
----------------------------------------
```

### Parsed Parameters
```
Parsed parameters:
  • username: jbloggs (Type: String)
  • limit: 5 (Type: Int32)
```

---

## Performance Tracking

### Execution Time
```
? Tool execution completed in 1234.56ms
```

Fast: < 500ms  
Normal: 500ms - 2000ms  
Slow: > 2000ms

### Progress for Multiple Items
```
?? Fetching details for 10 articles
  [1/10] Fetching article: ...
  [2/10] Fetching article: ...
  ...
? GetUserArticlesAsync completed: Fetched 10/145 articles
```

---

## Error Codes

| Code | Meaning | Common Cause |
|------|---------|--------------|
| `parse_error` | Invalid JSON | Malformed request |
| `invalid_params` | Missing/invalid parameters | Client error |
| `method_not_found` | Unknown method | Unsupported operation |
| `internal_error` | Server error | API failure, exception |

---

## Full Debug Session Example

```bash
# 1. Enable debug mode
set Logging__LogLevel__Default=Debug
set MCP_MODE=true
set ApiKey=your-key-here

# 2. Start server
cd "x:\...\Medium API MCP GH\Medium-API-MCP-GH"
dotnet run

# Expected output:
========================================
Medium MCP Server started. Listening for requests...
Protocol Version: 2024-11-05
Server Name: medium-stats
Server Version: 1.0.0
========================================
>>> SENDING SERVER INFO (Initialization)

# 3. Send test request (new PowerShell window)
echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}' | dotnet run

# Expected in first window:
========================================
RAW REQUEST RECEIVED:
{"jsonrpc":"2.0","id":1,"method":"initialize"...}
========================================
>>> INCOMING REQUEST: Method='initialize', ID='1'
...
? Initialization complete

# 4. Check VS Code Output panel
# Should see same logs if running via MCP
```

---

## Getting Help

### Include in Bug Reports:
1. **Full log output** from server start to error
2. **RAW REQUEST** that caused the issue
3. **Exception details** (type, message, stack trace)
4. **Environment info** (OS, .NET version, VS Code version)

### Format:
```
# Environment
- OS: Windows 11
- .NET SDK: 9.0.x
- VS Code: 1.x.x
- Copilot Extension: x.x.x

# Logs
========================================
Medium MCP Server started...
...
[error occurs]
!!! ERROR !!!
Exception Type: ...
Message: ...
Stack Trace: ...
```

---

## Pro Tips

### ?? Tip 1: Use Debug for Troubleshooting
When something doesn't work, immediately set `Debug` level to see everything.

### ?? Tip 2: Check Timestamps
Long gaps between log entries indicate slow operations or hangs.

### ?? Tip 3: Follow the Symbols
- `>>>` = Start looking here for request
- `!!!` = Problem occurred
- `?` = Operation failed
- `?` = Operation succeeded

### ?? Tip 4: RAW REQUEST/RESPONSE
When protocol issues occur, check the raw JSON for formatting problems.

### ?? Tip 5: Parameter Parsing
If tool execution fails immediately, check parameter parsing logs.

### ?? Tip 6: API Call Tracking
Each API call is logged with its method name - easy to spot which one failed.

---

## Related Documentation

- **TESTING-GUIDE.md** - Complete testing guide with examples
- **COPILOT-SETUP.md** - MCP server setup and configuration
- **README-MCP.md** - Detailed tool reference
- **API-QUICK-REFERENCE.md** - API coverage analysis

---

## Quick Test Commands

### Test 1: Check Server Starts
```bash
dotnet run
# Should see startup banner
```

### Test 2: Test MCP Mode
```bash
set MCP_MODE=true
dotnet run
# Should see "Listening for requests..."
```

### Test 3: Test with API Key
```bash
set MCP_MODE=true
set ApiKey=your-key-here
dotnet run
# Should start without errors
```

### Test 4: Enable Debug Output
```bash
set Logging__LogLevel__Default=Debug
set MCP_MODE=true
set ApiKey=your-key-here
dotnet run
# Should see detailed logs
```

---

## Summary

**Most Important:**
1. Set `Logging__LogLevel__Default=Debug` when troubleshooting
2. Look for `?` (success) and `?` (failure) symbols
3. Check `!!!` for errors
4. Follow `>>>` incoming requests to `<<<` outgoing responses
5. Review RAW REQUEST/RESPONSE for protocol issues

**Quick Diagnosis:**
- No startup banner? ? Server didn't start
- No "Listening for requests"? ? Not in MCP mode
- No initialize request? ? Client not connecting
- No tools list? ? Tool discovery failed
- Tool count not 11? ? Check McpToolHandler.GetToolDefinitions()
- Tool execution with `?`? ? Check exception details
- `401` error? ? API key issue
- `parse_error`? ? Check JSON formatting

---

## Tool Count Verification

**Expected tool count: 11**

Phase 0 (Original): 8 tools
- get_blog_statistics
- get_article_details
- get_user_articles
- search_articles
- search_tags
- get_top_articles_by_claps
- get_engagement_metrics
- get_publication_info

Phase 1 (Critical Features): 3 tools
- get_article_content
- get_user_info_by_id
- get_publication_articles

If you see a different count, check `McpToolHandler.cs` ? `GetToolDefinitions()` method.

---

**Document Version:** 2.0 (Phase 1 Complete)  
**Last Updated:** December 2024  
**Total Tools:** 11 (8 original + 3 Phase 1)  
**API Coverage:** 61% (11/18 methods)
