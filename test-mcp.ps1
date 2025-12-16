#!/usr/bin/env pwsh
# Test script for Medium MCP Server

Write-Host "Medium MCP Server Test Script" -ForegroundColor Cyan
Write-Host "==============================" -ForegroundColor Cyan
Write-Host ""

function Test-McpCommand {
    param(
        [string]$Command,
        [string]$Description
    )
    
    Write-Host "Testing: $Description" -ForegroundColor Yellow
    Write-Host "Command: $Command" -ForegroundColor Gray
    Write-Host ""
    
    $result = $Command | dotnet run -- --mcp
    Write-Host $result
    Write-Host ""
    Write-Host "---" -ForegroundColor Gray
    Write-Host ""
}

# Test 1: Initialize
$initRequest = @{
    jsonrpc = "2.0"
    id = 1
    method = "initialize"
    params = @{
        protocolVersion = "2024-11-05"
        clientInfo = @{
            name = "test-client"
            version = "1.0.0"
        }
    }
} | ConvertTo-Json -Compress

Test-McpCommand $initRequest "Initialize MCP Server"

# Test 2: List Tools
$listToolsRequest = @{
    jsonrpc = "2.0"
    id = 2
    method = "tools/list"
} | ConvertTo-Json -Compress

Test-McpCommand $listToolsRequest "List Available Tools"

# Test 3: Get Blog Statistics
# TODO: Change username to a different test user if needed
$getBlogStatsRequest = @{
    jsonrpc = "2.0"
    id = 3
    method = "tools/call"
    params = @{
        name = "get_blog_statistics"
        arguments = @{
            username = "jbloggs"
        }
    }
} | ConvertTo-Json -Compress

# TODO: Change username to a different test user if needed
Test-McpCommand $getBlogStatsRequest "Get Blog Statistics for jbloggs"

# Test 4: Get User Articles (Limited)
# TODO: Change username to a different test user if needed
$getUserArticlesRequest = @{
    jsonrpc = "2.0"
    id = 4
    method = "tools/call"
    params = @{
        name = "get_user_articles"
        arguments = @{
            username = "jbloggs"
            limit = 3
        }
    }
} | ConvertTo-Json -Compress

# TODO: Change username to a different test user if needed
Test-McpCommand $getUserArticlesRequest "Get 3 Articles for jbloggs"

Write-Host "Test script completed!" -ForegroundColor Green
