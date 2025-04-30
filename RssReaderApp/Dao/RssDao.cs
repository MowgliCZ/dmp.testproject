using Microsoft.EntityFrameworkCore;
using RssReaderApp.Models;

namespace RssReaderApp.Dao;

public class RssDao : DbContext {
    public RssDao(DbContextOptions<RssDao> options) : base(options) { }
    public DbSet<Feed> Feeds { get; set; }
    public DbSet<Article> Articles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Feed>()
            .HasMany(f => f.Articles)
            .WithOne(a => a.Feed)
            .HasForeignKey(a => a.FeedId);
    }
}
