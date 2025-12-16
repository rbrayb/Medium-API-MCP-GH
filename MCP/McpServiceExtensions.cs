using Microsoft.Extensions.DependencyInjection;

namespace Medium.Demos.ConsoleApp.MCP
{
    /// <summary>
    /// Extension methods for registering MCP server services
    /// </summary>
    public static class McpServiceExtensions
    {
        /// <summary>
        /// Add MCP Server for Medium API to the service collection
        /// </summary>
        public static IServiceCollection AddMediumMcpServer(this IServiceCollection services)
        {
            services.AddSingleton<McpServer>();
            services.AddSingleton<IMcpToolHandler, McpToolHandler>();
            services.AddSingleton<McpProtocolHandler>();
            
            return services;
        }
    }
}
