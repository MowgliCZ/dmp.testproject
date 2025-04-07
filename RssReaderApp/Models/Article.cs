namespace RssReaderApp.Models;

public class Article {
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Link { get; set; }
    public string? Description { get; set; }
    public DateTime PublishedDate { get; set; }

    public int FeedId { get; set; }
    public required Feed Feed { get; set; }
}