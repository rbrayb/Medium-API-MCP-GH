# ?? Medium MCP Server - Testing Guide

## Complete Guide to Testing Your MCP Server

This document provides comprehensive instructions for testing your Medium MCP Server implementation from the command line and other methods.

---

## ?? Table of Contents

1. [Quick Start](#quick-start)
2. [Starting the Server](#starting-the-server)
3. [Testing Methods](#testing-methods)
4. [Testing Individual Tools](#testing-individual-tools)
5. [Verification Checklist](#verification-checklist)
6. [Common Issues & Solutions](#common-issues--solutions)
7. [Example Test Sessions](#example-test-sessions)

---

## ?? Quick Start

### Minimum Steps to Test

```bash
# 1. Navigate to project
cd "D:\Src\VS2026\Medium API MCP"

# 2. Build the project
dotnet build

# 3. Run the server
dotnet run -- --mcp

# 4. Look for successful startup messages
# You should see: "? Built 16 tool definitions"
```

---

## ?? Starting the Server

### Basic Start (Recommended)

```bash
cd "D:\Src\VS2026\Medium API MCP"
dotnet run -- --mcp
```

**Expected Output:**
```
========================================
PHASE 1: CRITICAL FEATURES
========================================
========================================
PHASE 2: HIGH PRIORITY FEATURES
========================================
========================================
PHASE 3: MEDIUM/LOW PRIORITY FEATURES
========================================
? Built 16 tool definitions
MCP Server started and ready for requests...
```

### With Verbose Logging

```bash
dotnet run -- --mcp --verbose
```

### With Specific Configuration

```bash
# Using development environment
dotnet run --environment Development -- --mcp

# With custom appsettings
dotnet run --configuration Release -- --mcp
```

---

## ?? Testing Methods

### Method 1: MCP Inspector (Recommended) ?

**MCP Inspector** is the official tool for testing MCP servers interactively.

#### Installation

```bash
# Install globally
npm install -g @modelcontextprotocol/inspector

# Or use npx (no installation needed)
npx @modelcontextprotocol/inspector
```

#### Usage

```bash
# Start inspector with your server
npx @modelcontextprotocol/inspector dotnet run -- --mcp
```

This opens a web interface (usually `http://localhost:5173`) where you can:
- ? View all 16 available tools
- ? Test tool calls interactively with a UI
- ? See real-time request/response logs
- ? Debug tool parameters
- ? View JSON-RPC communication

#### Inspector Features

- **Tool List:** See all available tools with descriptions
- **Parameter Builder:** Fill in tool parameters with validation
- **Response Viewer:** See formatted JSON responses
- **Log Panel:** Monitor all communication
- **Error Details:** Debug failures with stack traces

---

### Method 2: Manual JSON-RPC via stdio

The MCP server communicates via stdio (standard input/output). You can send JSON-RPC messages manually.

#### Step-by-Step Process

**1. Start the server:**
```bash
dotnet run -- --mcp
```

**2. Send initialization request:**
```json
{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"test","version":"1.0"}}}
```

**3. List available tools:**
```json
{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}
```

**Expected Response:**
```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "result": {
    "tools": [
      {
        "name": "get_blog_statistics",
        "description": "Get comprehensive blog statistics...",
        "inputSchema": {...}
      },
      // ... 15 more tools
    ]
  }
}
```

**4. Call a tool:**
```json
{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"get_blog_statistics","arguments":{"username":"rbrayb"}}}
```

---

### Method 3: PowerShell Test Script

Create a reusable PowerShell script for testing.

#### Create `test-mcp.ps1`:

```powershell
# test-mcp.ps1 - Medium MCP Server Test Script

param(
    [string]$ProjectPath = "D:\Src\VS2026\Medium API MCP",
    [string]$ToolName = "get_blog_statistics",
    [hashtable]$Arguments = @{username="rbrayb"}
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Medium MCP Server - Test Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to project
Set-Location $ProjectPath
Write-Host "?? Project: $ProjectPath" -ForegroundColor Yellow
Write-Host ""

# Build the project
Write-Host "?? Building project..." -ForegroundColor Yellow
dotnet build --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "? Build successful" -ForegroundColor Green
Write-Host ""

# Start the server
Write-Host "?? Starting MCP Server..." -ForegroundColor Yellow
$process = Start-Process -FilePath "dotnet" `
    -ArgumentList "run","--","--mcp" `
    -WorkingDirectory $ProjectPath `
    -NoNewWindow -PassThru `
    -RedirectStandardInput `
    -RedirectStandardOutput `
    -RedirectStandardError

Write-Host "? Server started (PID: $($process.Id))" -ForegroundColor Green
Write-Host ""

# Wait for initialization
Write-Host "? Waiting for server initialization..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

# Create test JSON
$initRequest = @{
    jsonrpc = "2.0"
    id = 1
    method = "initialize"
    params = @{
        protocolVersion = "2024-11-05"
        capabilities = @{}
        clientInfo = @{
            name = "PowerShell Test"
            version = "1.0"
        }
    }
} | ConvertTo-Json -Compress

$listRequest = @{
    jsonrpc = "2.0"
    id = 2
    method = "tools/list"
    params = @{}
} | ConvertTo-Json -Compress

$toolRequest = @{
    jsonrpc = "2.0"
    id = 3
    method = "tools/call"
    params = @{
        name = $ToolName
        arguments = $Arguments
    }
} | ConvertTo-Json -Compress

Write-Host "?? Sending test requests..." -ForegroundColor Yellow
Write-Host ""

# Send requests (note: actual implementation would need proper stdio handling)
Write-Host "Test requests prepared:" -ForegroundColor Cyan
Write-Host "1. Initialize"
Write-Host "2. List tools"
Write-Host "3. Call tool: $ToolName"
Write-Host ""

Write-Host "?? For actual testing, use MCP Inspector:" -ForegroundColor Yellow
Write-Host "   npx @modelcontextprotocol/inspector dotnet run -- --mcp" -ForegroundColor White
Write-Host ""

# Cleanup
Write-Host "?? Stopping server..." -ForegroundColor Yellow
Stop-Process -Id $process.Id -Force
Write-Host "? Test script complete" -ForegroundColor Green
```

#### Run the script:

```powershell
# Default test
.\test-mcp.ps1

# Test specific tool
.\test-mcp.ps1 -ToolName "get_article_content" -Arguments @{article_id="e164ebf4fed1";format="markdown"}

# Custom project path
.\test-mcp.ps1 -ProjectPath "C:\MyProjects\MediumMCP"
```

---

### Method 4: GitHub Copilot Integration (Easiest) ?

The **most user-friendly** way to test is through GitHub Copilot Chat.

#### Setup

1. **Configure in VS Code settings** (`.vscode/settings.json`):

```json
{
  "mcp.servers": {
    "medium-api": {
      "command": "dotnet",
      "args": ["run", "--", "--mcp"],
      "cwd": "D:\\Src\\VS2026\\Medium API MCP",
      "env": {}
    }
  }
}
```

2. **Restart VS Code**

#### Usage

Open Copilot Chat and try:

```
"Get blog statistics for user rbrayb"
"Get article content for e164ebf4fed1"
"Show me fans for article e164ebf4fed1"
"Get followers for user 6601e21c1210"
"Search for articles about 'Verifiable credentials'"
```

Copilot will automatically:
- ? Start your MCP server
- ? Call the appropriate tool
- ? Parse the response
- ? Format the results

---

## ?? Testing Individual Tools

### All 16 Available Tools

#### Phase 0 Tools (Original)

**1. get_blog_statistics**
```json
{"jsonrpc":"2.0","id":1,"method":"tools/call","params":{"name":"get_blog_statistics","arguments":{"username":"rbrayb"}}}
```

**2. get_article_details**
```json
{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"get_article_details","arguments":{"article_id":"e164ebf4fed1"}}}
```

**3. get_user_articles**
```json
{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"get_user_articles","arguments":{"username":"rbrayb","limit":5}}}
```

**4. search_articles**
```json
{"jsonrpc":"2.0","id":4,"method":"tools/call","params":{"name":"search_articles","arguments":{"query":"Verifiable credentials","limit":3}}}
```

**5. search_tags**
```json
{"jsonrpc":"2.0","id":5,"method":"tools/call","params":{"name":"search_tags","arguments":{"query":"Custom policies"}}}
```

**6. get_top_articles_by_claps**
```json
{"jsonrpc":"2.0","id":6,"method":"tools/call","params":{"name":"get_top_articles_by_claps","arguments":{"username":"rbrayb","top_count":10}}}
```

**7. get_engagement_metrics**
```json
{"jsonrpc":"2.0","id":7,"method":"tools/call","params":{"name":"get_engagement_metrics","arguments":{"username":"rbrayb"}}}
```

**8. get_publication_info**
```json
{"jsonrpc":"2.0","id":8,"method":"tools/call","params":{"name":"get_publication_info","arguments":{"publication_id":"3a246e667a75"}}}
```

#### Phase 1 Tools (CRITICAL)

**9. get_article_content**
```json
{"jsonrpc":"2.0","id":9,"method":"tools/call","params":{"name":"get_article_content","arguments":{"article_id":"e164ebf4fed1","format":"markdown"}}}
```

**10. get_user_info_by_id**
```json
{"jsonrpc":"2.0","id":10,"method":"tools/call","params":{"name":"get_user_info_by_id","arguments":{"user_id":"6601e21c1210"}}}
```

**11. get_publication_articles**
```json
{"jsonrpc":"2.0","id":11,"method":"tools/call","params":{"name":"get_publication_articles","arguments":{"publication_slug_or_id":"the-new-control-plane","limit":5}}}
```

#### Phase 2 Tools (HIGH)

**12. get_recommended_articles**
```json
{"jsonrpc":"2.0","id":12,"method":"tools/call","params":{"name":"get_recommended_articles","arguments":{"article_id":"e164ebf4fed1","limit":5}}}
```

**13. get_article_responses**
```json
{"jsonrpc":"2.0","id":13,"method":"tools/call","params":{"name":"get_article_responses","arguments":{"article_id":"e164ebf4fed1"}}}
```

**14. get_related_articles**
```json
{"jsonrpc":"2.0","id":14,"method":"tools/call","params":{"name":"get_related_articles","arguments":{"article_id":"e164ebf4fed1","limit":5}}}
```

#### Phase 3 Tools (MEDIUM/LOW)

**15. get_article_fans**
```json
{"jsonrpc":"2.0","id":15,"method":"tools/call","params":{"name":"get_article_fans","arguments":{"article_id":"e164ebf4fed1","limit":10}}}
```

**16. get_user_followers**
```json
{"jsonrpc":"2.0","id":16,"method":"tools/call","params":{"name":"get_user_followers","arguments":{"user_id":"6601e21c1210","limit":10}}}
```

---

## ? Verification Checklist

### Pre-Flight Checks

- [ ] **Project builds successfully**
  ```bash
  dotnet build
  # Should complete with 0 errors
  ```

- [ ] **Configuration is valid**
  - [ ] `appsettings.json` exists
  - [ ] RapidAPI key is set
  - [ ] API key is valid

- [ ] **Dependencies are installed**
  ```bash
  dotnet restore
  # Should restore Medium.Client package
  ```

### Server Startup Verification

- [ ] **Server starts without errors**
  ```bash
  dotnet run -- --mcp
  # Should not throw exceptions
  ```

- [ ] **All tools are registered**
  ```
  Look for: "? Built 16 tool definitions"
  ```

- [ ] **MCP protocol initializes**
  ```
  Server should respond to initialize request
  ```

### Tool Execution Verification

- [ ] **Simple tool works** (get_blog_statistics)
- [ ] **Content extraction works** (get_article_content)
- [ ] **Audience tools work** (get_article_fans, get_user_followers)
- [ ] **No 401 Unauthorized errors** (API key is valid)
- [ ] **Response format is correct** (valid JSON)

### Log Verification

Check logs for:
- [ ] ?? API calls being made
- [ ] ? Successful operations
- [ ] ? No unexpected errors
- [ ] Tool execution completion messages

---

## ?? Common Issues & Solutions

### Issue 1: Server Won't Start

**Symptoms:**
```
Application startup exception
```

**Solutions:**
```bash
# Check build errors
dotnet build

# Check runtime errors
dotnet run

# Verify .NET version
dotnet --version  # Should be 9.0 or higher
```

---

### Issue 2: "API key not configured"

**Symptoms:**
```
ERROR: RapidAPI key not configured
```

**Solutions:**

1. **Check `appsettings.json`:**
```json
{
  "MediumApi": {
    "RapidApiKey": "your-actual-key-here"
  }
}
```

2. **Check environment variables:**
```bash
# Windows PowerShell
$env:MediumApi__RapidApiKey = "your-key"

# Windows CMD
set MediumApi__RapidApiKey=your-key

# Linux/Mac
export MediumApi__RapidApiKey="your-key"
```

3. **Verify key in RapidAPI dashboard:**
   - Login to RapidAPI
   - Check Medium API subscription
   - Copy correct API key

---

### Issue 3: "401 Unauthorized"

**Symptoms:**
```
? Error: Response status code does not indicate success: 401 (Unauthorized)
```

**Solutions:**

1. **Verify API key is correct**
2. **Check RapidAPI subscription is active**
3. **Ensure API key has Medium API access**
4. **Check for expired subscription**

---

### Issue 4: "Tool not found"

**Symptoms:**
```
Unknown tool: get_blog_stats
```

**Solutions:**

1. **Check exact tool name** (case-sensitive):
   - ? `get_blog_statistics`
   - ? `get_blog_stats`

2. **List available tools:**
```json
{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}
```

---

### Issue 5: Slow Response Times

**Symptoms:**
- Tools take long time to respond
- Timeout errors

**Solutions:**

1. **Use limit parameters:**
```json
{"limit": 5}  // Instead of fetching all results
```

2. **Check RapidAPI rate limits**
3. **Consider caching frequent requests**
4. **Monitor API quota usage**

---

### Issue 6: "Parameter required" Errors

**Symptoms:**
```
? Parameter 'username' is required
```

**Solutions:**

1. **Check parameter names match exactly:**
   - ? `username`
   - ? `user_name`

2. **Ensure parameters are provided:**
```json
{
  "name": "get_blog_statistics",
  "arguments": {
    "username": "rbrayb"  // ? Required parameter
  }
}
```

3. **Check tool definition for required parameters**

---

## ?? Example Test Sessions

### Example 1: Complete Startup Test

```bash
PS D:\Src\VS2026\Medium API MCP> dotnet run -- --mcp

# Expected Output:
========================================
PHASE 1: CRITICAL FEATURES
========================================
========================================
PHASE 2: HIGH PRIORITY FEATURES
========================================
========================================
PHASE 3: MEDIUM/LOW PRIORITY FEATURES
========================================
? Built 16 tool definitions
info: Medium.Demos.ConsoleApp.MCP.McpServer[0]
      ?? GetBlogStatisticsAsync - Username: rbrayb
info: Medium.Demos.ConsoleApp.MCP.McpServer[0]
      ? GetBlogStatisticsAsync completed successfully
```

---

### Example 2: Using MCP Inspector

```bash
PS D:\Src\VS2026\Medium API MCP> npx @modelcontextprotocol/inspector dotnet run -- --mcp

# Browser opens to http://localhost:5173
# UI shows:
# - 16 tools listed
# - Interactive parameter forms
# - Real-time logs
```

**Test in Inspector:**
1. Select "get_blog_statistics"
2. Enter username: "rbrayb"
3. Click "Execute"
4. See response in 1-2 seconds
5. Check logs panel for details

---

### Example 3: Testing Multiple Tools

```bash
# Test workflow: User ? Articles ? Engagement

# 1. Get user info
"Get blog statistics for rbrayb"
# Result: 792 followers, 145 articles

# 2. Get user's articles
"Get user articles for rbrayb, limit 5"
# Result: Top 5 articles listed

# 3. Get article details
"Get article details for e164ebf4fed1"
# Result: 5 claps, 1 response, 1 voter

# 4. Get article content
"Get article content for e164ebf4fed1 in markdown"
# Result: Full markdown content

# 5. Get article fans
"Get fans for article e164ebf4fed1"
# Result: Users who clapped

# 6. Get user followers
"Get followers for user 6601e21c1210"
# Result: Follower list
```

---

### Example 4: Error Handling Test

```bash
# Test with invalid article ID
{"jsonrpc":"2.0","id":1,"method":"tools/call","params":{"name":"get_article_details","arguments":{"article_id":"invalid123"}}}

# Expected Response:
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "{\"Success\":false,\"ErrorMessage\":\"Article not found or invalid ID\"}"
      }
    ]
  }
}
```

---

## ?? Performance Testing

### Response Time Benchmarks

| Tool | Typical Response | Acceptable | Slow |
|------|------------------|------------|------|
| get_blog_statistics | 1-2s | <3s | >5s |
| get_article_content | 2-3s | <5s | >10s |
| get_user_articles (limit=5) | 3-5s | <8s | >15s |
| get_article_fans (limit=10) | 5-8s | <12s | >20s |
| search_articles (limit=3) | 2-4s | <6s | >12s |

### Load Testing

```bash
# Test multiple sequential requests
for i in {1..10}; do
    echo "Request $i"
    # Send tool call
    sleep 1
done
```

---

## ?? Debugging Tips

### Enable Detailed Logging

```bash
# Set environment variable
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run -- --mcp
```

### Monitor API Calls

Look for these log patterns:
```
?? Calling Medium API: Users.GetInfoByUsernameAsync
? User info retrieved: ID=abc123, Name=John Doe
?? Calling Medium API: Users.GetArticlesIdByUserIdAsync
? Retrieved 145 article IDs
```

### Check Error Stack Traces

When errors occur:
```
? Error in GetBlogStatisticsAsync for user: invalid_user
Exception Type: HttpRequestException
Message: Response status code does not indicate success: 404 (Not Found)
Stack Trace: [full stack trace shown]
```

---

## ?? Additional Resources

### Documentation Files
- `README.md` - Project overview
- `QUICKSTART.md` - Getting started guide
- `PHASE1-IMPLEMENTATION.md` - Phase 1 features
- `PHASE2-IMPLEMENTATION.md` - Phase 2 features
- `PHASE3-IMPLEMENTATION.md` - Phase 3 features
- `COMPLETE-JOURNEY.md` - Full timeline
- `API-GAP-ANALYSIS.md` - API coverage analysis

### External Resources
- [MCP Protocol Specification](https://modelcontextprotocol.io/)
- [Medium API Documentation](https://rapidapi.com/nishujain199719-vgIfuFHZxVZ/api/medium2)
- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/)

---

## ?? Quick Reference

### Essential Commands

```bash
# Start server
dotnet run -- --mcp

# Build project
dotnet build

# Clean build
dotnet clean && dotnet build

# Run tests
dotnet test

# Check .NET version
dotnet --version

# Restore packages
dotnet restore
```

### Tool Testing Template

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "tools/call",
  "params": {
    "name": "TOOL_NAME",
    "arguments": {
      "param1": "value1",
      "param2": "value2"
    }
  }
}
```

---

## ? Final Checklist

Before considering testing complete:

- [ ] Server starts without errors
- [ ] All 16 tools are registered
- [ ] At least one tool executes successfully
- [ ] API key is validated (no 401 errors)
- [ ] Logs show expected patterns
- [ ] Response format is valid JSON
- [ ] Error handling works (test invalid input)
- [ ] Performance is acceptable (<5s typical response)

---

## ?? Getting Help

If you encounter issues:

1. **Check logs** for error messages
2. **Verify configuration** (API key, settings)
3. **Review documentation** for examples
4. **Test with MCP Inspector** for interactive debugging
5. **Check API quota** on RapidAPI dashboard

---

## ?? Success Criteria

Your testing is successful when:

? Server starts cleanly  
? All 16 tools are available  
? Sample tool calls return valid data  
? No authentication errors  
? Response times are acceptable  
? Error handling works properly  

**Your Medium MCP Server with 83% API coverage (15/18 methods) is ready for production!** ??

---

**Document Version:** 1.0  
**Last Updated:** December 2025  
**Server Version:** Phase 3 Complete (83% API Coverage)  
**Total Tools:** 16 active MCP tools
