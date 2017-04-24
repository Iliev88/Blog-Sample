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
    public class CommentController : Controller
    {
        private BlogDbContext database = new BlogDbContext();

        // GET: Comment
        public ActionResult Index()
        {
            var comments = database.Comments
                .Include(c => c.Article)
                .OrderByDescending(c => c.Date);

            return View(comments.ToList());
        }

        public ActionResult ListCommentsOnArticle(int? id)
        {
            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Include(a => a.Author)
                    .Include(t => t.Tags)
                    .Include(c => c.Comments)
                    .Where(a => a.Id == id)
                    .FirstOrDefault();

                return View(article);
            }
        }

        // GET: Comment/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comment = database.Comments.Find(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        // GET: Comment/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comment = new Comment();

            comment.ArticleId = (int)id;

            return View(comment);

        }

        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,VisitorComment,ArticleId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                database.Comments.Add(comment);
                database.SaveChanges();
                
                return RedirectToAction("ListCommentsOnArticle", new { id = comment.ArticleId });
            }

            ViewBag.ArticleId = new SelectList(database.Articles, "Id", "Title", comment.ArticleId);

            return View(comment);
        }

        // GET: Comment/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = database.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArticleId = new SelectList(database.Articles, "Id", "Title", comment.ArticleId);
            return View(comment);
        }

        // POST: Comment/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,VisitorComment,ArticleId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                database.Entry(comment).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArticleId = new SelectList(database.Articles, "Id", "Title", comment.ArticleId);
            return View(comment);
        }

        // GET: Comment/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = database.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comment/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = database.Comments.Find(id);
            database.Comments.Remove(comment);
            database.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
