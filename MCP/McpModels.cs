namespace Medium.Demos.ConsoleApp.MCP
{
    // Base result class
    public abstract class McpResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // Blog statistics result
    public class BlogStatisticsResult : McpResult
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int FollowersCount { get; set; }
        public int ArticleCount { get; set; }
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
        public string? TwitterUsername { get; set; }

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            return $@"Blog Statistics for {FullName} (@{Username})
- User ID: {UserId}
- Followers: {FollowersCount:N0}
- Articles: {ArticleCount:N0}
- Twitter: @{TwitterUsername ?? "N/A"}
- Bio: {Bio ?? "N/A"}";
        }
    }

    // Article details result
    public class ArticleDetailsResult : McpResult
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public int Claps { get; set; }
        public int ResponsesCount { get; set; }
        public int Voters { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<string> Topics { get; set; } = new();

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            return $@"Article: {Title}
- Claps: {Claps:N0} | Responses: {ResponsesCount:N0} | Voters: {Voters:N0}
- Published: {PublishedDate:yyyy-MM-dd}
- Tags: {string.Join(", ", Tags)}
- URL: {Url}";
        }
    }

    // User articles result
    public class UserArticlesResult : McpResult
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int TotalArticleCount { get; set; }
        public List<ArticleDetailsResult> Articles { get; set; } = new();

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            var articlesText = string.Join("\n\n", Articles.Select((a, i) => $"{i + 1}. {a}"));
            return $@"Articles by {FullName} (@{Username})
Total Articles: {TotalArticleCount:N0}
Showing: {Articles.Count:N0}

{articlesText}";
        }
    }

    // Search articles result
    public class SearchArticlesResult : McpResult
    {
        public string Query { get; set; } = string.Empty;
        public int TotalResultsCount { get; set; }
        public List<ArticleDetailsResult> Articles { get; set; } = new();

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            var articlesText = string.Join("\n\n", Articles.Select((a, i) => $"{i + 1}. {a}"));
            return $@"Search Results for: '{Query}'
Total Results: {TotalResultsCount:N0}
Showing: {Articles.Count:N0}

{articlesText}";
        }
    }

    // Tag information result
    public class TagInfoResult
    {
        public string Tag { get; set; } = string.Empty;
        public int ArticlesCount { get; set; }
        public int AuthorsCount { get; set; }

        public override string ToString()
        {
            return $"#{Tag} - {ArticlesCount:N0} articles by {AuthorsCount:N0} authors";
        }
    }

    // Tag search result
    public class TagSearchResult : McpResult
    {
        public string Query { get; set; } = string.Empty;
        public List<TagInfoResult> Tags { get; set; } = new();

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            var tagsText = string.Join("\n", Tags.Select((t, i) => $"{i + 1}. {t}"));
            return $@"Tag Search Results for: '{Query}'
Found {Tags.Count:N0} tag(s)

{tagsText}";
        }
    }

    // Top articles result
    public class TopArticlesResult : McpResult
    {
        public string Username { get; set; } = string.Empty;
        public string Criteria { get; set; } = string.Empty;
        public List<ArticleDetailsResult> Articles { get; set; } = new();

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            var articlesText = string.Join("\n\n", Articles.Select((a, i) => $"{i + 1}. {a}"));
            return $@"Top Articles by {Criteria} for @{Username}
Showing: {Articles.Count:N0}

{articlesText}";
        }
    }

    // Engagement metrics result
    public class EngagementMetricsResult : McpResult
    {
        public string Username { get; set; } = string.Empty;
        public int TotalArticles { get; set; }
        public int TotalClaps { get; set; }
        public int TotalResponses { get; set; }
        public int TotalVoters { get; set; }
        public int AverageClapsPerArticle { get; set; }
        public int AverageResponsesPerArticle { get; set; }
        public int AverageVotersPerArticle { get; set; }

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            return $@"Engagement Metrics for @{Username}
Total Articles: {TotalArticles:N0}

Aggregate Metrics:
- Total Claps: {TotalClaps:N0}
- Total Responses: {TotalResponses:N0}
- Total Voters: {TotalVoters:N0}

Average per Article:
- Claps: {AverageClapsPerArticle:N0}
- Responses: {AverageResponsesPerArticle:N0}
- Voters: {AverageVotersPerArticle:N0}";
        }
    }

    // Publication information result
    public class PublicationInfoResult : McpResult
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Tagline { get; set; }
        public string? Description { get; set; }
        public List<string> Tags { get; set; } = new();
        public int Followers { get; set; }
        public string? InstagramUsername { get; set; }
        public string? FacebookPageName { get; set; }
        public string? TwitterUsername { get; set; }
        public string? Url { get; set; }
        public string? Slug { get; set; }
        public string? Creator { get; set; }
        public List<string>? Editors { get; set; }

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            return $@"Publication: {Name}
- ID: {Id}
- Tagline: {Tagline ?? "N/A"}
- Followers: {Followers:N0}
- Tags: {string.Join(", ", Tags)}
- URL: {Url ?? "N/A"}
- Twitter: @{TwitterUsername ?? "N/A"}
- Instagram: @{InstagramUsername ?? "N/A"}
- Facebook: {FacebookPageName ?? "N/A"}
- Description: {Description ?? "N/A"}";
        }
    }

    // ===== PHASE 1: CRITICAL FEATURES =====

    // Article content result (markdown, HTML, or text)
    public class ArticleContentResult : McpResult
    {
        public string ArticleId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Format { get; set; } = string.Empty; // "markdown", "html", or "text"
        public string Content { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public int ContentLength { get; set; }

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            var preview = Content.Length > 200 
                ? Content.Substring(0, 200) + "..." 
                : Content;

            return $@"Article Content: {Title}
- Format: {Format}
- Length: {ContentLength:N0} characters
- Published: {PublishedDate:yyyy-MM-dd}
- URL: {Url}

Preview:
{preview}";
        }
    }

    // User information result (by ID lookup)
    public class UserInfoResult : McpResult
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int FollowersCount { get; set; }
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
        public string? TwitterUsername { get; set; }
        public string? ProfileUrl { get; set; }

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            return $@"User: {FullName} (@{Username})
- User ID: {UserId}
- Followers: {FollowersCount:N0}
- Twitter: @{TwitterUsername ?? "N/A"}
- Profile: {ProfileUrl ?? "N/A"}
- Bio: {Bio ?? "N/A"}";
        }
    }

    // Publication articles result
    public class PublicationArticlesResult : McpResult
    {
        public string PublicationId { get; set; } = string.Empty;
        public string PublicationName { get; set; } = string.Empty;
        public string? PublicationSlug { get; set; }
        public int TotalArticleCount { get; set; }
        public List<ArticleDetailsResult> Articles { get; set; } = new();

        public override string ToString()
        {
            if (!Success)
                return $"Error: {ErrorMessage}";

            var articlesText = string.Join("\n\n", Articles.Select((a, i) => $"{i + 1}. {a}"));
            return $@"Articles from Publication: {PublicationName}
- Publication ID: {PublicationId}
- Slug: {PublicationSlug ?? "N/A"}
- Total Articles: {TotalArticleCount:N0}
- Showing: {Articles.Count:N0}

{articlesText}";
        }
    }
}
