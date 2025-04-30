using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RssReaderApp.Dao;
using RssReaderApp.Models;

namespace RssReaderApp.Dao;

public class ArticleDao {
    private readonly RssDao _context;

    public ArticleDao(RssDao context) {
        _context = context;
    }

    public async Task Add(int feedId, List<Article> articles) {
        var feed = await _context.Feeds.FindAsync(feedId);
        if (feed != null) {
            foreach (var article in articles) {
                article.FeedId = feedId;
                _context.Articles.Add(article);
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Article>> List(int feedId, int page = 1, int pageSize = 10) {
        return await _context.Articles
            .Where(a => a.FeedId == feedId)
            .OrderByDescending(a => a.PublishedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Article>> ListByDate(
    int feedId,
    DateTime? fromDate,
    DateTime? toDate,
    int page = 1,
    int pageSize = 10) {

        var query = _context.Articles.Where(a => a.FeedId == feedId);

        if (fromDate.HasValue && toDate.HasValue) {
            var from = fromDate.Value.Date;
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(a => a.PublishedDate >= from && a.PublishedDate < to);
        } else if (fromDate.HasValue) {
            var from = fromDate.Value.Date;
            query = query.Where(a => a.PublishedDate >= from);
        } else if (toDate.HasValue) {
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(a => a.PublishedDate < to);
        }

        return await query
            .OrderByDescending(a => a.PublishedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize + 1)
            .ToListAsync();
    }


    public async Task<int> Count(
    int feedId,
    DateTime? fromDate,
    DateTime? toDate,
    string searchTerm) {
        var query = _context.Articles.Where(a => a.FeedId == feedId);

        if (!string.IsNullOrWhiteSpace(searchTerm)) {
            query = query.Where(a => a.Title.ToLower().Contains(searchTerm.ToLower()));
        }

        if (fromDate.HasValue && toDate.HasValue) {
            var from = fromDate.Value.Date;
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(a => a.PublishedDate >= from && a.PublishedDate < to);
        } else if (fromDate.HasValue) {
            var from = fromDate.Value.Date;
            query = query.Where(a => a.PublishedDate >= from);
        } else if (toDate.HasValue) {
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(a => a.PublishedDate < to);
        }

        return await query.CountAsync();
    }

    public async Task<List<Article>> SearchArticles(
        int feedId,
        DateTime? fromDate,
        DateTime? toDate,
        string searchTerm,
        int page = 1,
        int pageSize = 10) {

        var query = _context.Articles.Where(a => a.FeedId == feedId);

        if (!string.IsNullOrWhiteSpace(searchTerm)) {
            query = query.Where(a => a.Title.ToLower().Contains(searchTerm.ToLower()));
        }

        if (fromDate.HasValue && toDate.HasValue) {
            var from = fromDate.Value.Date;
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(a => a.PublishedDate >= from && a.PublishedDate < to);
        } else if (fromDate.HasValue) {
            var from = fromDate.Value.Date;
            query = query.Where(a => a.PublishedDate >= from);
        } else if (toDate.HasValue) {
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(a => a.PublishedDate < to);
        }

        return await query
            .OrderByDescending(a => a.PublishedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }




    public async Task<bool> Exists(Expression<Func<Article, bool>> predicate) {
        return await _context.Articles.AnyAsync(predicate);
    }
}