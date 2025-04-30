using Microsoft.EntityFrameworkCore;
using RssReaderApp.Dao;
using RssReaderApp.Models;

namespace RssReaderApp.Dao;

public class FeedDao {
    private readonly RssDao _context;

    public FeedDao(RssDao context) {
        _context = context;
    }

    public async Task Create(Feed feed) {
        _context.Feeds.Add(feed);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Feed feed) {
        _context.Feeds.Update(feed);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Feed>> List() {
        return await _context.Feeds.ToListAsync();
    }

    public async Task<Feed> GetById(int id) {
        return await _context.Feeds
            .Include(f => f.Articles)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task DeleteById(int id) {
        var feed = await _context.Feeds.FindAsync(id);
        if (feed != null) {
            _context.Feeds.Remove(feed);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteManyById(List<int> ids) {
        var feedsToDelete = await _context.Feeds.Where(f => ids.Contains(f.Id)).ToListAsync();
        _context.Feeds.RemoveRange(feedsToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Feed>> SearchByName(string term) {
        if (string.IsNullOrWhiteSpace(term))
            return new List<Feed>();

        return await _context.Feeds
            .Where(f => f.Name.ToLower().Contains(term.ToLower()))
            .OrderBy(f => f.Name)
            .Take(10)
            .ToListAsync();
    }
}
