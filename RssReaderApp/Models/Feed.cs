namespace RssReaderApp.Models;

public class Feed {
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public DateTime? LastUpdated { get; set; }
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}