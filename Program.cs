using Medium.Client;
using Medium.Client.Abstractions;
using Medium.Client.DependencyResolver;
using Medium.Domain.Article;
using Medium.Domain.List;
using Medium.Domain.Platform;
using Medium.Domain.Publication;
using Medium.Domain.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Medium.Demos.ConsoleApp.MCP;

namespace Medium.Demos.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Check if running in MCP mode (for GitHub Copilot integration)
            bool isMcpMode = Environment.GetEnvironmentVariable("MCP_MODE") == "true" || 
                             args.Contains("--mcp");

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Ensure appsettings.json is loaded from the correct location
                    var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                    
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    
                    // Add environment variables with Medium: prefix mapping
                    config.AddEnvironmentVariables();
                    
                    // Support direct ApiKey environment variable
                    var apiKey = Environment.GetEnvironmentVariable("ApiKey");
                    if (!string.IsNullOrEmpty(apiKey))
                    {
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            { "Medium:ApiKey", apiKey }
                        });
                    }
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    // Enable Debug logging in MCP mode for diagnostics
                    logging.SetMinimumLevel(isMcpMode ? LogLevel.Debug : LogLevel.Information);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddMediumClient(defaultRetryPolicy: true);
                    services.AddMediumMcpServer();
                })
                .Build();

            if (isMcpMode)
            {
                // Run as MCP server for GitHub Copilot
                await RunMcpServerAsync(host);
            }
            else
            {
                // Run as console application
                await RunConsoleAppAsync(host);
            }
        }

        static async Task RunMcpServerAsync(IHost host)
        {
            var protocolHandler = host.Services.GetRequiredService<McpProtocolHandler>();
            
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            await protocolHandler.RunAsync(cts.Token);
        }

        static async Task RunConsoleAppAsync(IHost host)
        {
            IMediumClient mediumClient = host.Services.GetRequiredService<IMediumClient>();

            // TODO: Replace "jbloggs" with a valid Medium username for testing
            UserInfo userInfo = await mediumClient.Users.GetInfoByUsernameAsync("jbloggs");
            Console.WriteLine($"User {userInfo.Fullname} with ID {userInfo.Id} and {userInfo.FollowersCount} followers found!");

            string userId = userInfo.Id;

            IEnumerable<string> articleIds = await mediumClient.Users.GetArticlesIdByUserIdAsync(userId);
            ListArticles listArticles = new ListArticles
            {
                Id = userId,
                Articles = articleIds
            };

            int articleCount = listArticles.Articles.Count();
            Console.WriteLine($"\nUser {userInfo.Fullname} has {articleCount} articles.\n");

            int loopCount = 0;

            // Enumerate through the articles and display their details
            foreach (var articleId in listArticles.Articles) // Assuming 'Articles' is the collection property
            {
                ArticleInfo articleInfo = await mediumClient.Articles.GetInfoByIdAsync(articleId);
                Console.WriteLine($"Article ID: {articleInfo.Id}");
                Console.WriteLine($"Title: {articleInfo.Title}");
                Console.WriteLine($"Claps: {articleInfo.Claps}");
                Console.WriteLine($"Published date: {articleInfo.PublishedDate}");
                Console.WriteLine($"Responses count: {articleInfo.ResponsesCount}");
                Console.WriteLine($"Voters: {articleInfo.Voters}");
                Console.WriteLine($"URL: {articleInfo.Url}");

                foreach (var tag in articleInfo.Tags)
                {
                    Console.WriteLine($"Tag: {tag}");
                }

                Console.WriteLine();

                loopCount++;

                if (loopCount == 4)
                    break;

            }

            Console.WriteLine("\nSearching for articles around VC\n");

            ISearchClient searchClient = host.Services.GetRequiredService<ISearchClient>();
            IEnumerable<string> searchIds = await searchClient.GetArticlesByQueryAsync("Verifiable credentials");

            int searchCount = searchIds.Count();
            Console.WriteLine($"\nSearch returned {searchCount} articles.\n");

            int loopSearchCount = 0;

            foreach (var searchId in searchIds)
            {
                ArticleInfo articleInfo = await mediumClient.Articles.GetInfoByIdAsync(searchId);
                Console.WriteLine($"Article ID: {articleInfo.Id}");
                Console.WriteLine($"Title: {articleInfo.Title}");
                Console.WriteLine($"Claps: {articleInfo.Claps}");
                Console.WriteLine($"Published date: {articleInfo.PublishedDate}");
                Console.WriteLine($"Responses count: {articleInfo.ResponsesCount}");
                Console.WriteLine($"Voters: {articleInfo.Voters}");
                Console.WriteLine($"URL: {articleInfo.Url}");
                Console.WriteLine();

                loopSearchCount++;

                if (loopSearchCount == 4)
                    break;
            }

            Console.WriteLine("\nSearching for tag called Entra External ID\n");

            IEnumerable<string> tagIds = await searchClient.GetTagsByQueryAsync("Entra External ID");

            int tagCount = tagIds.Count();
            Console.WriteLine($"\nSearch returned {tagCount} tags.\n");

            IPlatformClient platformClient = host.Services.GetRequiredService<IPlatformClient>();

            foreach (var tagId in tagIds)
            {
                TagInfo tagInfo = await platformClient.GetTagInfoAsync(tagId);

                Console.WriteLine($"Articles count: {tagInfo.ArticlesCount}");
                Console.WriteLine($"Authors count: {tagInfo.AuthorsCount}");
                Console.WriteLine($"Tag: {tagInfo.Tag}");
            }

            Console.WriteLine("\nSearching for tag called Custom policies\n");

            tagIds = await searchClient.GetTagsByQueryAsync("Custom policies");

            tagCount = tagIds.Count();
            Console.WriteLine($"\nSearch returned {tagCount} tags.\n");

            platformClient = host.Services.GetRequiredService<IPlatformClient>();

            foreach (var tagId in tagIds)
            {
                TagInfo tagInfo = await platformClient.GetTagInfoAsync(tagId);

                Console.WriteLine($"Articles count: {tagInfo.ArticlesCount}");
                Console.WriteLine($"Authors count: {tagInfo.AuthorsCount}");
                Console.WriteLine($"Tag: {tagInfo.Tag}");
            }

            // TODO: Replace "123456" with a valid Medium publication ID for testing
            Console.WriteLine("\n\nGetting Publication Info for publication ID: 123456\n");

            try
            {
                // TODO: Replace "123456" with a valid Medium publication ID for testing
                var publicationInfo = await mediumClient.Publications.GetInfoByIdAsync("123456");

                Console.WriteLine($"Publication ID: {publicationInfo.Id}");
                Console.WriteLine($"Name: {publicationInfo.Name}");
                Console.WriteLine($"Tagline: {publicationInfo.TagLine}");
                Console.WriteLine($"Description: {publicationInfo.Description}");
                Console.WriteLine($"Followers: {publicationInfo.Followers}");
                Console.WriteLine($"URL: {publicationInfo.Url}");
                Console.WriteLine($"Slug: {publicationInfo.Slug}");
                Console.WriteLine($"Twitter: @{publicationInfo.TwitterUsername}");
                Console.WriteLine($"Instagram: @{publicationInfo.InstagramUsername}");
                Console.WriteLine($"Facebook Page: {publicationInfo.FacebookPageName}");
                Console.WriteLine($"Creator ID: {publicationInfo.CreatorId}");
                
                if (publicationInfo.Tags != null && publicationInfo.Tags.Any())
                {
                    Console.WriteLine($"Tags: {string.Join(", ", publicationInfo.Tags)}");
                }
                
                if (publicationInfo.Editors != null && publicationInfo.Editors.Any())
                {
                    Console.WriteLine($"Editors: {string.Join(", ", publicationInfo.Editors)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting publication info: {ex.Message}");
            }
        }
    }
}
