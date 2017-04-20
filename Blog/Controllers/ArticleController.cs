using Blog.Extensions;
using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        public ActionResult Index()
        {
            return RedirectToAction("ListLast");
        }

        // GET: Article/List
        public ActionResult List()
        {
            using (var database = new BlogDbContext())
            {
                var articles = database.Articles
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .OrderByDescending(a => a.Date)
                    .ToList();
                
                return View(articles);
            }
        }

        // GET: Article/List Last 4
        public ActionResult ListLast()
        {
            using (var database = new BlogDbContext())
            {
                var articles = database.Articles
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .OrderByDescending(a => a.Date)
                    .Take(4)
                    .ToList();

                return View(articles);
            }
        }

        // GET: Article/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                this.AddNotification("No article ID provided.", NotificationType.ERROR);
                return RedirectToAction("List");
            }

            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .Include(a => a.Comments)
                    .First();

                if (article == null)
                {
                    this.AddNotification("Such an article does not exist", NotificationType.ERROR);
                    return RedirectToAction("List");
                }

                return View(article);
            }
        }

        // GET: Article/Create
        [Authorize]
        public ActionResult Create()
        {
            using (var database = new BlogDbContext())
            {
                var model = new ArticleViewModel();
                model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(model);
            }
        }

        // POST: Article/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    var authorId = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    var article = new Article(authorId, model.Title, model.Content, model.CategoryId);
                    article.Date = DateTime.Now;

                    this.SetArticleTags(article, model, database);

                    database.Articles.Add(article);
                    database.SaveChanges();

                    this.AddNotification("Article was successfully created.", NotificationType.SUCCESS);
                    return RedirectToAction("List");
                }
            }

            return View(model);
        }

        private void SetArticleTags(Article article, ArticleViewModel model, BlogDbContext database)
        {
            var tagsStrings = model.Tags
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLower())
                .Distinct();

            article.Tags.Clear();

            foreach (var tagString in tagsStrings)
            {
                var tag = database.Tags.FirstOrDefault(t => t.Name.Equals(tagString));

                if (tag == null)
                {
                    tag = new Tag() { Name = tagString };
                    database.Tags.Add(tag);
                }

                article.Tags.Add(tag);
            }
        }

        // GET: Article/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                this.AddNotification("No article ID provided.", NotificationType.ERROR);
                return RedirectToAction("List");
            }

            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Category)
                    .First();

                if (!IsUserAuthorizedToEdit(article))
                {
                    this.AddNotification("You have no rights to perform that!", NotificationType.ERROR);
                    return RedirectToAction("List");
                }

                ViewBag.TagsString = string.Join(", ", article.Tags.Select(t => t.Name));

                if  (article == null)
                {
                    this.AddNotification("Such an article does not exist.", NotificationType.ERROR);
                    return RedirectToAction("List");
                }

                return View(article);
            }
        }

        // POST: Article/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                this.AddNotification("No article ID provided.", NotificationType.ERROR);
                return RedirectToAction("List");
            }

            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (article == null)
                {
                    this.AddNotification("Such an article does not exist.", NotificationType.ERROR);
                    return RedirectToAction("List");
                }

                database.Articles.Remove(article);
                database.SaveChanges();

                this.AddNotification("The article was deleted.", NotificationType.SUCCESS);
                return RedirectToAction("List");
            }
        }

        // GET: Article/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                this.AddNotification("No article ID provided.", NotificationType.ERROR);
                return RedirectToAction("List");
            }

            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .First();

                if (!IsUserAuthorizedToEdit(article))
                {
                    this.AddNotification("You have no rights to perform that!", NotificationType.ERROR);
                    return RedirectToAction("List");
                }

                if (article == null)
                {
                    this.AddNotification("Such an article does not exist.", NotificationType.ERROR);
                    return RedirectToAction("List");
                }

                var model = new ArticleViewModel();
                model.Id = article.Id;
                model.Title = article.Title;
                model.Content = article.Content;
                model.CategoryId = article.CategoryId;
                model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();

                model.Tags = string.Join(", ", article.Tags.Select(t => t.Name));

                return View(model);
            }
        }

        // POST: Article/Edit
        [HttpPost]
        public ActionResult Edit(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    var article = database.Articles
                        .FirstOrDefault(a => a.Id == model.Id);

                    article.Title = model.Title;
                    article.Content = model.Content;
                    article.CategoryId = model.CategoryId;
                    this.SetArticleTags(article, model, database);

                    database.Entry(article).State = EntityState.Modified;
                    database.SaveChanges();

                    this.AddNotification("Article was edited.", NotificationType.SUCCESS);
                    return RedirectToAction("List");
                }
            }
            
            return View(model);
        }

        private bool IsUserAuthorizedToEdit(Article article)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }
    }
}