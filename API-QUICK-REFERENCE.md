# Medium API - Quick Reference


## Implemented Tools (11 Total)

### Phase 0: Original Tools (8 tools)
1. **get_blog_statistics** - User overview with followers and article count
2. **get_article_details** - Article metadata and engagement
3. **get_user_articles** - List user's articles with optional limit
4. **search_articles** - Search articles by query string
5. **search_tags** - Find tags with article/author counts
6. **get_top_articles_by_claps** - Best performing articles
7. **get_engagement_metrics** - Comprehensive analytics
8. **get_publication_info** - Publication metadata and stats

### Phase 1: Critical Features (3 tools) ? COMPLETE
9. **get_article_content** - Full article content (markdown/HTML/text)
10. **get_user_info_by_id** - Fast user lookup by ID
11. **get_publication_articles** - List publication's articles

---


- 3 tools added (total: 11)
- Article content access (markdown/HTML/text)
- User ID lookup (performance improvement)
- Publication article listing

---

## API Method Usage by Tool

| Tool | Primary API Method | Secondary Methods |
|------|-------------------|-------------------|
| get_blog_statistics | GetInfoByUsernameAsync | GetArticlesIdByUserIdAsync |
| get_article_details | GetInfoByIdAsync | - |
| get_user_articles | GetInfoByUsernameAsync | GetArticlesIdByUserIdAsync, GetInfoByIdAsync |
| search_articles | GetArticlesByQueryAsync | GetInfoByIdAsync |
| search_tags | GetTagsByQueryAsync | GetTagInfoAsync |
| get_top_articles_by_claps | GetInfoByUsernameAsync | GetArticlesIdByUserIdAsync, GetInfoByIdAsync |
| get_engagement_metrics | GetInfoByUsernameAsync | GetArticlesIdByUserIdAsync, GetInfoByIdAsync |
| get_publication_info | GetInfoByIdAsync (Pubs) | - |
| get_article_content | GetDetailMarkdown/Html/TextByIdAsync | GetInfoByIdAsync |
| get_user_info_by_id | GetInfoByIdAsync (Users) | - |
| get_publication_articles | GetPublicationIdAsync | GetArticlesByPublicationIdAsync, GetInfoByIdAsync |

---


## Performance Notes

| Operation | Typical Time | Notes |
|-----------|--------------|-------|
| get_blog_statistics | 1-2s | Fast - 2 API calls |
| get_article_content | 2-3s | Moderate - content retrieval |
| get_user_articles (all) | 3-10s | Slow - multiple API calls |
| get_user_articles (limit=5) | 2-4s | Moderate - limited fetches |
| search_articles | 2-4s | Moderate - search + details |
| get_publication_articles | 3-8s | Moderate to slow - depends on limit |

**Tip:** Use `limit` parameters to improve response times for list operations.

---

## API Coverage by Category

| Category | Implemented | Total | Coverage |
|----------|-------------|-------|----------|
| Users API | 2 methods | 7 methods | 29% |
| Articles API | 4 methods | 9 methods | 44% |
| Publications API | 3 methods | 4 methods | 75% |
| Search API | 2 methods | 2 methods | 100% ? |
| Platform API | 1 method | 1+ methods | 100% ? |
| **Overall** | **11 methods** | **18+ methods** | **61%** |

---


