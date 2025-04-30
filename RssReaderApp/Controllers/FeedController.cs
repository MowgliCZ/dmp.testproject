using System;
using System.Xml;
using System.ServiceModel.Syndication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RssReaderApp.Models;
using RssReaderApp.Dao;
using System.Runtime.CompilerServices;

namespace RssReaderApp.Controllers;

public class FeedController : Controller {
    private readonly FeedDao _feedDao;
    private readonly ArticleDao _articleDao;

    public FeedController(FeedDao feedDao, ArticleDao articleDao) {
        _feedDao = feedDao;
        _articleDao = articleDao;
    }

    [HttpGet]
    public IActionResult Add() {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(Feed feed) {
        if (ModelState.IsValid) {
            await _feedDao.Create(feed);
            return RedirectToAction("Index");
        }
        return View(feed);
    }

    [HttpGet]
    public async Task<IActionResult> Index() {
        var feeds = await _feedDao.List();
        return View(feeds);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id) {
        await _feedDao.DeleteById(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMultiple(List<int> ids) {
        await _feedDao.DeleteManyById(ids);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, DateTime? fromDate, DateTime? toDate, string searchTerm, int page = 1, int pageSize = 10) {
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);

        await _reloadArticlesIfNeeded(id);

        var feed = await _feedDao.GetById(id);

        if (feed == null) {
            return NotFound();
        }

        var articles = await _articleDao.SearchArticles(id, fromDate, toDate, searchTerm, page, pageSize);

        var totalArticles = await _articleDao.Count(id, fromDate, toDate, searchTerm);

        var viewModel = new FeedDetailsViewModel {
            Feed = feed,
            Articles = articles,
            Page = page,
            PageSize = pageSize,
            TotalArticles = totalArticles,
            SearchTerm = searchTerm,
            FromDate = fromDate,
            ToDate = toDate
        };

        return View(viewModel);
    }

    private async Task _reloadArticlesIfNeeded(int id) {
        var feed = await _feedDao.GetById(id);
        if (feed == null) return;

        if (feed.LastUpdated.HasValue && feed.LastUpdated.Value.AddMinutes(30) > DateTime.Now) {
            return;
        }

        await ReloadArticles(id);
    }

    [HttpPost]
    public async Task<IActionResult> ReloadArticles(int id) {
        var feed = await _feedDao.GetById(id);
        if (feed == null) {
            return NotFound();
        }

        try {
            using var reader = XmlReader.Create(feed.Url);
            var syndicationFeed = SyndicationFeed.Load(reader);

            var articlesToAdd = new List<Article>();

            foreach (var item in syndicationFeed.Items) {
                var link = item.Links.FirstOrDefault()?.Uri.ToString();
                bool exists = await _articleDao.Exists(a => a.Link == link);
                if (!exists) {
                    articlesToAdd.Add(new Article {
                        Title = item.Title.Text,
                        Link = link,
                        Description = item.Summary?.Text,
                        PublishedDate = item.PublishDate.DateTime,
                        FeedId = id,
                        Feed = feed
                    });
                }
            }

            await _articleDao.Add(id, articlesToAdd);

            feed.LastUpdated = DateTime.Now;
            await _feedDao.Update(feed);

            return RedirectToAction("Details", new { id });
        } catch (Exception ex) {
            return StatusCode(500, "Chyba pøi naèítání èlánkù: " + ex.Message);
        }
    }

    [HttpGet]
    public async Task<JsonResult> SearchFeeds(string term) {
        var feeds = await _feedDao.SearchByName(term);
        return Json(feeds.Select(f => new { f.Id, f.Name }));
    }
}
