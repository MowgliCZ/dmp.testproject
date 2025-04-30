namespace RssReaderApp.Models;

public class FeedDetailsViewModel {
    public required Feed Feed { get; set; }
    public required List<Article> Articles { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalArticles { get; set; }
    public string SearchTerm { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
