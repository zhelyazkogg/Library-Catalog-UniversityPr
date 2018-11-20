using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LibCatalog.Models;
using PagedList;


namespace LibCatalog.Controllers
{
    public class BooksController : Controller
    {
        private LibCatalogContext db = new LibCatalogContext();

        // GET: Books
        public ActionResult Index(int? page, string searchString, string currentFilter, string sortOrder, string writerSearch, string genreSearch)
        {
            var books = db.Books.Include(b => b.Genre).Include(b => b.Writer);

            var GenreLst = new List<string>();

            var GenreQry = from d in db.Books
                           orderby d.Genre.GenreName
                           select d.Genre.GenreName;

            GenreLst.AddRange(GenreQry.Distinct());

            ViewBag.genreSearch = new SelectList(GenreLst);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "Title" : "";
            ViewBag.GenreSortParm = String.IsNullOrEmpty(sortOrder) ? "Genre" : "";

            

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(writerSearch))
            {
                books = books.Where(b => b.Writer.FirstName.Contains(writerSearch));
            }

            if (!string.IsNullOrEmpty(genreSearch))
            {
                books = books.Where(x => x.Genre.GenreName == genreSearch);
            }

            switch (sortOrder)
            {
                case "Title":
                    books = books.OrderByDescending(b => b.Title);
                    break;
                default:  // Name ascending 
                    books = books.OrderBy(b => b.Title);
                    break;
            }

            int pageNumber = (page ?? 1);
            int pageSize = 3;
            return View(books.ToPagedList(pageNumber, pageSize));

        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "GenreName");
            ViewBag.WriterId = new SelectList(db.Writers, "WriterId", "FirstName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookId,Title,ReleaseDate,WriterId,GenreId,Description")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "GenreName", book.GenreId);
            ViewBag.WriterId = new SelectList(db.Writers, "WriterId", "FirstName", book.WriterId);
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "GenreName", book.GenreId);
            ViewBag.WriterId = new SelectList(db.Writers, "WriterId", "FirstName", book.WriterId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookId,Title,ReleaseDate,WriterId,GenreId,Description")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "GenreName", book.GenreId);
            ViewBag.WriterId = new SelectList(db.Writers, "WriterId", "FirstName", book.WriterId);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
