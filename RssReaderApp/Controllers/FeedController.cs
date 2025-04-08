using System;
using System.Xml;
using System.ServiceModel.Syndication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RssReaderApp.Models;
using System.Runtime.CompilerServices;

namespace RssReaderApp.Controllers;

public class FeedController : Controller {
    private readonly RssDao _context;

    public FeedController(RssDao context) {
        _context = context;
    }

    [HttpGet]
    public IActionResult Add() {
        return View();
    }

    [HttpPost]
    public IActionResult Add(Feed feed) {
        if (ModelState.IsValid) {
            _context.Feeds.Add(feed);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(feed);
    }

    public IActionResult Index() {
        var feeds = _context.Feeds?.ToList();
        return View(feeds);
    }

    [HttpPost]
    public IActionResult Delete(int id) {
        var feed = _context.Feeds.Find(id);
        if (feed != null) {
            _context.Feeds.Remove(feed);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeleteMultiple(List<int> ids) {
        foreach (var id in ids) {
            var feed = _context.Feeds.Find(id);
            if (feed != null) {
                _context.Feeds.Remove(feed);
            }
        }
        _context.SaveChanges();
        return Ok();
    }

    public IActionResult Details(int id, DateTime? fromDate, DateTime? toDate) {
        ReloadArticlesIfNeeded(id);

        var feed = _context.Feeds.Include(f => f.Articles).FirstOrDefault(f => f.Id == id);

        if (feed == null) {
            return NotFound();
        }

        var articles = feed.Articles.AsQueryable();

        if (fromDate.HasValue)
            articles = articles.Where(a => a.PublishedDate >= fromDate.Value);

        if (toDate.HasValue)
            articles = articles.Where(a => a.PublishedDate <= toDate.Value);

        var model = new FeedDetailsViewModel {
            Feed = feed,
            Articles = articles.ToList()
        };

        ViewBag.Articles = articles.ToList();
        ViewBag.FromDate = fromDate;
        ViewBag.ToDate = toDate;

        return View(feed);
    }

    private void ReloadArticlesIfNeeded(int id) {
        var feed = _context.Feeds.Find(id);
        if (feed == null) return;

        if (feed.LastUpdated.HasValue && feed.LastUpdated.Value.AddMinutes(30) > DateTime.Now) {
            return;
        }

        ReloadArticles(id);
    }

    [HttpPost]
    public IActionResult ReloadArticles(int id) {
        Console.WriteLine($"ReloadArticles called for feed ID: {id}");
        var feed = _context.Feeds.Find(id);
        if (feed == null) {
            return NotFound();
        }

        try {
            using var reader = XmlReader.Create(feed.Url);
            var syndicationFeed = SyndicationFeed.Load(reader);
            Console.WriteLine($"Feed: {feed}");

            foreach (var item in syndicationFeed.Items) {
                var link = item.Links.FirstOrDefault()?.Uri.ToString();
                if (!string.IsNullOrEmpty(link) && !_context.Articles.Any(a => a.Link == link)) {
                    _context.Articles.Add(new Article {
                        Title = item.Title.Text,
                        Link = link,
                        Description = item.Summary?.Text,
                        PublishedDate = item.PublishDate.DateTime,
                        FeedId = id,
                        Feed = feed
                    });
                }
            }

            feed.LastUpdated = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("Details", new { id });
        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Chyba pøi naèítání èlánkù: " + ex.Message);
        }
    }
}
