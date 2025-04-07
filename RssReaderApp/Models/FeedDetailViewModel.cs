namespace RssReaderApp.Models;

public class FeedDetailsViewModel {
	public required Feed Feed { get; set; }
	public required List<Article> Articles { get; set; }
}
