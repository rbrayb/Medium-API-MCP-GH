using Medium.Client;
using Medium.Client.Abstractions;
using Medium.Domain.Article;
using Medium.Domain.Platform;
using Medium.Domain.Publication;
using Medium.Domain.User;
using Microsoft.Extensions.Logging;

namespace Medium.Demos.ConsoleApp.MCP
{
    /// <summary>
    /// MCP Server for Medium API integration with GitHub Copilot
    /// Provides natural language access to Medium blog statistics
    /// </summary>
    public class McpServer
    {
        private readonly IMediumClient _mediumClient;
        private readonly ISearchClient _searchClient;
        private readonly IPlatformClient _platformClient;
        private readonly ILogger<McpServer> _logger;

        public McpServer(
            IMediumClient mediumClient,
            ISearchClient searchClient,
            IPlatformClient platformClient,
            ILogger<McpServer> logger)
        {
            _mediumClient = mediumClient;
            _searchClient = searchClient;
            _platformClient = platformClient;
            _logger = logger;
        }

        /// <summary>
        /// Get blog statistics for a user including follower count and article count
        /// </summary>
        public async Task<BlogStatisticsResult> GetBlogStatisticsAsync(string username)
        {
            _logger.LogInformation("? GetBlogStatisticsAsync - Username: {Username}", username);
            
            try
            {
                _logger.LogDebug("  ? Calling Medium API: Users.GetInfoByUsernameAsync");
                var userInfo = await _mediumClient.Users.GetInfoByUsernameAsync(username);
                _logger.LogDebug("  ? User info retrieved: ID={UserId}, Name={FullName}, Followers={Followers}", 
                    userInfo.Id, userInfo.Fullname, userInfo.FollowersCount);

                _logger.LogDebug("  ? Calling Medium API: Users.GetArticlesIdByUserIdAsync");
                var articleIds = await _mediumClient.Users.GetArticlesIdByUserIdAsync(userInfo.Id);
                _logger.LogDebug("  ? Retrieved {Count} article IDs", articleIds.Count());

                var result = new BlogStatisticsResult
                {
                    Success = true,
                    Username = username,
                    FullName = userInfo.Fullname,
                    UserId = userInfo.Id,
                    FollowersCount = userInfo.FollowersCount,
                    ArticleCount = articleIds.Count(),
                    Bio = userInfo.Bio,
                    ImageUrl = userInfo.ImageUrl,
                    TwitterUsername = userInfo.TwitterUsername
                };

                _logger.LogInformation("? GetBlogStatisticsAsync completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in GetBlogStatisticsAsync for user: {Username}", username);
                return new BlogStatisticsResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get detailed article information including claps, responses, and voters
        /// </summary>
        public async Task<ArticleDetailsResult> GetArticleDetailsAsync(string articleId)
        {
            _logger.LogInformation("? GetArticleDetailsAsync - Article ID: {ArticleId}", articleId);
            
            try
            {
                _logger.LogDebug("  ? Calling Medium API: Articles.GetInfoByIdAsync");
                var articleInfo = await _mediumClient.Articles.GetInfoByIdAsync(articleId);
                _logger.LogDebug("  ? Article retrieved: Title='{Title}', Claps={Claps}, Responses={Responses}", 
                    articleInfo.Title, articleInfo.Claps, articleInfo.ResponsesCount);

                var result = new ArticleDetailsResult
                {
                    Success = true,
                    Id = articleInfo.Id,
                    Title = articleInfo.Title,
                    Subtitle = articleInfo.Subtitle,
                    Claps = articleInfo.Claps,
                    ResponsesCount = articleInfo.ResponsesCount,
                    Voters = articleInfo.Voters,
                    Url = articleInfo.Url,
                    PublishedDate = articleInfo.PublishedDate,
                    Tags = articleInfo.Tags?.ToList() ?? new List<string>(),
                    Topics = articleInfo.Topics?.ToList() ?? new List<string>()
                };

                _logger.LogInformation("? GetArticleDetailsAsync completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in GetArticleDetailsAsync for article ID: {ArticleId}", articleId);
                return new ArticleDetailsResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get all articles for a user with optional limit
        /// </summary>
        public async Task<UserArticlesResult> GetUserArticlesAsync(string username, int? limit = null)
        {
            _logger.LogInformation("? GetUserArticlesAsync - Username: {Username}, Limit: {Limit}", 
                username, limit?.ToString() ?? "none");
            
            try
            {
                _logger.LogDebug("  ? Calling Medium API: Users.GetInfoByUsernameAsync");
                var userInfo = await _mediumClient.Users.GetInfoByUsernameAsync(username);
                _logger.LogDebug("  ? User info retrieved: ID={UserId}", userInfo.Id);

                _logger.LogDebug("  ? Calling Medium API: Users.GetArticlesIdByUserIdAsync");
                var articleIds = await _mediumClient.Users.GetArticlesIdByUserIdAsync(userInfo.Id);
                _logger.LogDebug("  ? Retrieved {Count} article IDs", articleIds.Count());

                var articlesToFetch = limit.HasValue 
                    ? articleIds.Take(limit.Value) 
                    : articleIds;
                
                var fetchCount = articlesToFetch.Count();
                _logger.LogDebug("  ? Fetching details for {Count} articles", fetchCount);

                var articles = new List<ArticleDetailsResult>();
                var index = 0;
                foreach (var articleId in articlesToFetch)
                {
                    index++;
                    _logger.LogDebug("    [{Index}/{Total}] Fetching article: {ArticleId}", 
                        index, fetchCount, articleId);
                    
                    var articleDetails = await GetArticleDetailsAsync(articleId);
                    if (articleDetails.Success)
                    {
                        articles.Add(articleDetails);
                    }
                    else
                    {
                        _logger.LogWarning("    ? Failed to fetch article {ArticleId}: {Error}", 
                            articleId, articleDetails.ErrorMessage);
                    }
                }

                var result = new UserArticlesResult
                {
                    Success = true,
                    Username = username,
                    FullName = userInfo.Fullname,
                    TotalArticleCount = articleIds.Count(),
                    Articles = articles
                };

                _logger.LogInformation("? GetUserArticlesAsync completed: Fetched {Fetched}/{Total} articles", 
                    articles.Count, articleIds.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in GetUserArticlesAsync for user: {Username}", username);
                return new UserArticlesResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Search for articles by query string
        /// </summary>
        public async Task<SearchArticlesResult> SearchArticlesAsync(string query, int? limit = null)
        {
            _logger.LogInformation("? SearchArticlesAsync - Query: '{Query}', Limit: {Limit}", 
                query, limit?.ToString() ?? "none");
            
            try
            {
                _logger.LogDebug("  ? Calling Medium API: Search.GetArticlesByQueryAsync");
                var articleIds = await _searchClient.GetArticlesByQueryAsync(query);
                _logger.LogDebug("  ? Found {Count} matching articles", articleIds.Count());

                var articlesToFetch = limit.HasValue 
                    ? articleIds.Take(limit.Value) 
                    : articleIds;
                
                var fetchCount = articlesToFetch.Count();
                _logger.LogDebug("  ? Fetching details for {Count} articles", fetchCount);

                var articles = new List<ArticleDetailsResult>();
                var index = 0;
                foreach (var articleId in articlesToFetch)
                {
                    index++;
                    _logger.LogDebug("    [{Index}/{Total}] Fetching article: {ArticleId}", 
                        index, fetchCount, articleId);
                    
                    var articleDetails = await GetArticleDetailsAsync(articleId);
                    if (articleDetails.Success)
                    {
                        articles.Add(articleDetails);
                    }
                    else
                    {
                        _logger.LogWarning("    ? Failed to fetch article {ArticleId}: {Error}", 
                            articleId, articleDetails.ErrorMessage);
                    }
                }

                var result = new SearchArticlesResult
                {
                    Success = true,
                    Query = query,
                    TotalResultsCount = articleIds.Count(),
                    Articles = articles
                };

                _logger.LogInformation("? SearchArticlesAsync completed: Fetched {Fetched}/{Total} articles", 
                    articles.Count, articleIds.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in SearchArticlesAsync for query: '{Query}'", query);
                return new SearchArticlesResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get tag information including article and author counts
        /// </summary>
        public async Task<TagSearchResult> SearchTagsAsync(string query)
        {
            _logger.LogInformation("? SearchTagsAsync - Query: '{Query}'", query);
            
            try
            {
                _logger.LogDebug("  ? Calling Medium API: Search.GetTagsByQueryAsync");
                var tagIds = await _searchClient.GetTagsByQueryAsync(query);
                _logger.LogDebug("  ? Found {Count} matching tags", tagIds.Count());

                var tags = new List<TagInfoResult>();
                var index = 0;
                foreach (var tagId in tagIds)
                {
                    index++;
                    _logger.LogDebug("  [{Index}/{Total}] Fetching tag: {TagId}", 
                        index, tagIds.Count(), tagId);
                    
                    var tagInfo = await _platformClient.GetTagInfoAsync(tagId);
                    tags.Add(new TagInfoResult
                    {
                        Tag = tagInfo.Tag,
                        ArticlesCount = tagInfo.ArticlesCount,
                        AuthorsCount = tagInfo.AuthorsCount
                    });
                }

                var result = new TagSearchResult
                {
                    Success = true,
                    Query = query,
                    Tags = tags
                };

                _logger.LogInformation("? SearchTagsAsync completed: Retrieved {Count} tags", tags.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in SearchTagsAsync for query: '{Query}'", query);
                return new TagSearchResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get top performing articles for a user based on claps
        /// </summary>
        public async Task<TopArticlesResult> GetTopArticlesByClapsAsync(string username, int topCount = 10)
        {
            _logger.LogInformation("? GetTopArticlesByClapsAsync - Username: {Username}, TopCount: {TopCount}", 
                username, topCount);
            
            try
            {
                _logger.LogDebug("  ? Getting all user articles");
                var userArticles = await GetUserArticlesAsync(username);
                
                if (!userArticles.Success)
                {
                    _logger.LogError("  ? Failed to get user articles: {Error}", userArticles.ErrorMessage);
                    return new TopArticlesResult
                    {
                        Success = false,
                        ErrorMessage = userArticles.ErrorMessage
                    };
                }

                _logger.LogDebug("  ? Sorting articles by claps and taking top {TopCount}", topCount);
                var topArticles = userArticles.Articles
                    .OrderByDescending(a => a.Claps)
                    .Take(topCount)
                    .ToList();

                _logger.LogDebug("  ? Top articles:");
                for (int i = 0; i < topArticles.Count; i++)
                {
                    _logger.LogDebug("    {Rank}. '{Title}' - {Claps} claps", 
                        i + 1, topArticles[i].Title, topArticles[i].Claps);
                }

                var result = new TopArticlesResult
                {
                    Success = true,
                    Username = username,
                    Criteria = "Claps",
                    Articles = topArticles
                };

                _logger.LogInformation("? GetTopArticlesByClapsAsync completed: Returned {Count} articles", 
                    topArticles.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in GetTopArticlesByClapsAsync for user: {Username}", username);
                return new TopArticlesResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get comprehensive engagement metrics for a user's articles
        /// </summary>
        public async Task<EngagementMetricsResult> GetEngagementMetricsAsync(string username)
        {
            _logger.LogInformation("? GetEngagementMetricsAsync - Username: {Username}", username);
            
            try
            {
                _logger.LogDebug("  ? Getting all user articles");
                var userArticles = await GetUserArticlesAsync(username);
                
                if (!userArticles.Success)
                {
                    _logger.LogError("  ? Failed to get user articles: {Error}", userArticles.ErrorMessage);
                    return new EngagementMetricsResult
                    {
                        Success = false,
                        ErrorMessage = userArticles.ErrorMessage
                    };
                }

                var articles = userArticles.Articles;
                var totalArticles = articles.Count;

                _logger.LogDebug("  ? Calculating engagement metrics for {Count} articles", totalArticles);

                if (totalArticles == 0)
                {
                    _logger.LogWarning("  ? No articles found for user");
                    return new EngagementMetricsResult
                    {
                        Success = true,
                        Username = username,
                        TotalArticles = 0
                    };
                }

                var totalClaps = articles.Sum(a => a.Claps);
                var totalResponses = articles.Sum(a => a.ResponsesCount);
                var totalVoters = articles.Sum(a => a.Voters);

                _logger.LogDebug("  ? Metrics calculated:");
                _logger.LogDebug("    Total Claps: {TotalClaps}", totalClaps);
                _logger.LogDebug("    Total Responses: {TotalResponses}", totalResponses);
                _logger.LogDebug("    Total Voters: {TotalVoters}", totalVoters);
                _logger.LogDebug("    Avg Claps/Article: {AvgClaps}", totalClaps / totalArticles);
                _logger.LogDebug("    Avg Responses/Article: {AvgResponses}", totalResponses / totalArticles);
                _logger.LogDebug("    Avg Voters/Article: {AvgVoters}", totalVoters / totalArticles);

                var result = new EngagementMetricsResult
                {
                    Success = true,
                    Username = username,
                    TotalArticles = totalArticles,
                    TotalClaps = totalClaps,
                    TotalResponses = totalResponses,
                    TotalVoters = totalVoters,
                    AverageClapsPerArticle = totalClaps / totalArticles,
                    AverageResponsesPerArticle = totalResponses / totalArticles,
                    AverageVotersPerArticle = totalVoters / totalArticles
                };

                _logger.LogInformation("? GetEngagementMetricsAsync completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in GetEngagementMetricsAsync for user: {Username}", username);
                return new EngagementMetricsResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get publication information including name, tagline, followers, and social media links
        /// </summary>
        public async Task<PublicationInfoResult> GetPublicationInfoAsync(string publicationId)
        {
            _logger.LogInformation("?? GetPublicationInfoAsync - Publication ID: {PublicationId}", publicationId);
            
            try
            {
                _logger.LogDebug("  ?? Calling Medium API: Publications.GetInfoByIdAsync");
                var publicationInfo = await _mediumClient.Publications.GetInfoByIdAsync(publicationId);
                _logger.LogDebug("  ? Publication retrieved: Name='{Name}', Followers={Followers}", 
                    publicationInfo.Name, publicationInfo.Followers);

                var result = new PublicationInfoResult
                {
                    Success = true,
                    Id = publicationInfo.Id,
                    Name = publicationInfo.Name,
                    Tagline = publicationInfo.TagLine,
                    Description = publicationInfo.Description,
                    Tags = publicationInfo.Tags?.ToList() ?? new List<string>(),
                    Followers = publicationInfo.Followers,
                    InstagramUsername = publicationInfo.InstagramUsername,
                    FacebookPageName = publicationInfo.FacebookPageName,
                    TwitterUsername = publicationInfo.TwitterUsername,
                    Url = publicationInfo.Url,
                    Slug = publicationInfo.Slug,
                    Creator = publicationInfo.CreatorId,
                    Editors = publicationInfo.Editors?.ToList()
                };

                _logger.LogInformation("? GetPublicationInfoAsync completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in GetPublicationInfoAsync for publication ID: {PublicationId}", publicationId);
                return new PublicationInfoResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // ========================================
        // PHASE 1: CRITICAL FEATURES
        // ========================================

        /// <summary>
        /// Get full article content in specified format (markdown, HTML, or text)
        /// </summary>
        /// <param name="articleId">Article ID</param>
        /// <param name="format">Content format: "markdown", "html", or "text" (default: markdown)</param>
        public async Task<ArticleContentResult> GetArticleContentAsync(string articleId, string format = "markdown")
        {
            _logger.LogInformation("?? GetArticleContentAsync - Article ID: {ArticleId}, Format: {Format}", articleId, format);
            
            try
            {
                // First get basic article info
                _logger.LogDebug("  ?? Calling Medium API: Articles.GetInfoByIdAsync");
                var articleInfo = await _mediumClient.Articles.GetInfoByIdAsync(articleId);
                _logger.LogDebug("  ? Article info retrieved: Title='{Title}'", articleInfo.Title);

                string content;
                string actualFormat = format.ToLowerInvariant();

                // Get content in requested format
                switch (actualFormat)
                {
                    case "markdown":
                    case "md":
                        _logger.LogDebug("  ?? Calling Medium API: Articles.GetDetailMarkdownByIdAsync");
                        content = await _mediumClient.Articles.GetDetailMarkdownByIdAsync(articleId);
                        actualFormat = "markdown";
                        break;

                    case "html":
                        _logger.LogDebug("  ?? Calling Medium API: Articles.GetDetailHtmlByIdAsync");
                        content = await _mediumClient.Articles.GetDetailHtmlByIdAsync(articleId);
                        actualFormat = "html";
                        break;

                    case "text":
                    case "txt":
                        _logger.LogDebug("  ?? Calling Medium API: Articles.GetDetailTextByIdAsync");
                        content = await _mediumClient.Articles.GetDetailTextByIdAsync(articleId);
                        actualFormat = "text";
                        break;

                    default:
                        _logger.LogWarning("  ?? Invalid format '{Format}', defaulting to markdown", format);
                        content = await _mediumClient.Articles.GetDetailMarkdownByIdAsync(articleId);
                        actualFormat = "markdown";
                        break;
                }

                _logger.LogDebug("  ? Content retrieved: {Length} characters", content.Length);

                var result = new ArticleContentResult
                {
                    Success = true,
                    ArticleId = articleId,
                    Title = articleInfo.Title,
                    Subtitle = articleInfo.Subtitle,
                    Format = actualFormat,
                    Content = content,
                    Url = articleInfo.Url,
                    PublishedDate = articleInfo.PublishedDate,
                    ContentLength = content.Length
                };

                _logger.LogInformation("? GetArticleContentAsync completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in GetArticleContentAsync for article ID: {ArticleId}", articleId);
                return new ArticleContentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get user information by user ID (faster than username lookup)
        /// </summary>
        /// <param name="userId">Medium user ID</param>
        public async Task<UserInfoResult> GetUserInfoByIdAsync(string userId)
        {
            _logger.LogInformation("?? GetUserInfoByIdAsync - User ID: {UserId}", userId);
            
            try
            {
                _logger.LogDebug("  ?? Calling Medium API: Users.GetInfoByIdAsync");
                var userInfo = await _mediumClient.Users.GetInfoByIdAsync(userId);
                _logger.LogDebug("  ? User info retrieved: Username={Username}, Name={FullName}, Followers={Followers}", 
                    userInfo.Username, userInfo.Fullname, userInfo.FollowersCount);

                var result = new UserInfoResult
                {
                    Success = true,
                    UserId = userInfo.Id,
                    Username = userInfo.Username,
                    FullName = userInfo.Fullname,
                    FollowersCount = userInfo.FollowersCount,
                    Bio = userInfo.Bio,
                    ImageUrl = userInfo.ImageUrl,
                    TwitterUsername = userInfo.TwitterUsername,
                    ProfileUrl = $"https://medium.com/@{userInfo.Username}"
                };

                _logger.LogInformation("? GetUserInfoByIdAsync completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in GetUserInfoByIdAsync for user ID: {UserId}", userId);
                return new UserInfoResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get all articles published in a specific publication
        /// </summary>
        /// <param name="publicationSlugOrId">Publication slug (name) or publication ID</param>
        /// <param name="limit">Maximum number of articles to return (optional)</param>
        public async Task<PublicationArticlesResult> GetPublicationArticlesAsync(string publicationSlugOrId, int? limit = null)
        {
            _logger.LogInformation("?? GetPublicationArticlesAsync - Publication: {Publication}, Limit: {Limit}", 
                publicationSlugOrId, limit?.ToString() ?? "none");
            
            try
            {
                string publicationId;
                
                // Check if input is a publication ID (starts with letters/numbers) or a slug
                // If it doesn't look like an ID, try to resolve it as a slug
                if (publicationSlugOrId.Length < 10 || publicationSlugOrId.Contains("-"))
                {
                    _logger.LogDebug("  ?? Calling Medium API: Publications.GetPublicationIdAsync (resolving slug)");
                    var pubId = await _mediumClient.Publications.GetPublicationIdAsync(publicationSlugOrId);
                    publicationId = pubId.Id;
                    _logger.LogDebug("  ? Resolved publication ID: {PublicationId}", publicationId);
                }
                else
                {
                    publicationId = publicationSlugOrId;
                    _logger.LogDebug("  ?? Using provided publication ID: {PublicationId}", publicationId);
                }

                // Get publication info
                _logger.LogDebug("  ?? Calling Medium API: Publications.GetInfoByIdAsync");
                var publicationInfo = await _mediumClient.Publications.GetInfoByIdAsync(publicationId);
                _logger.LogDebug("  ? Publication info retrieved: Name='{Name}'", publicationInfo.Name);

                // Get articles for this publication
                _logger.LogDebug("  ?? Calling Medium API: Publications.GetArticlesByPublicationIdAsync");
                var articleIds = await _mediumClient.Publications.GetArticlesByPublicationIdAsync(publicationId);
                _logger.LogDebug("  ? Retrieved {Count} article IDs", articleIds.Count());

                var articlesToFetch = limit.HasValue 
                    ? articleIds.Take(limit.Value) 
                    : articleIds;
                
                var fetchCount = articlesToFetch.Count();
                _logger.LogDebug("  ?? Fetching details for {Count} articles", fetchCount);

                var articles = new List<ArticleDetailsResult>();
                var index = 0;
                foreach (var articleId in articlesToFetch)
                {
                    index++;
                    _logger.LogDebug("    [{Index}/{Total}] Fetching article: {ArticleId}", 
                        index, fetchCount, articleId);
                    
                    var articleDetails = await GetArticleDetailsAsync(articleId);
                    if (articleDetails.Success)
                    {
                        articles.Add(articleDetails);
                    }
                    else
                    {
                        _logger.LogWarning("    ?? Failed to fetch article {ArticleId}: {Error}", 
                            articleId, articleDetails.ErrorMessage);
                    }
                }

                var result = new PublicationArticlesResult
                {
                    Success = true,
                    PublicationId = publicationId,
                    PublicationName = publicationInfo.Name,
                    PublicationSlug = publicationInfo.Slug,
                    TotalArticleCount = articleIds.Count(),
                    Articles = articles
                };

                _logger.LogInformation("? GetPublicationArticlesAsync completed: Fetched {Fetched}/{Total} articles", 
                    articles.Count, articleIds.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error in GetPublicationArticlesAsync for publication: {Publication}", publicationSlugOrId);
                return new PublicationArticlesResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
