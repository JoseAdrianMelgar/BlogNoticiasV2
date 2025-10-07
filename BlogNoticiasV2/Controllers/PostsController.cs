using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BlogNoticiasV2.Models;
using Microsoft.AspNet.Identity;

namespace BlogNoticiasV2.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            var posts = _db.Posts.Include(p => p.Categoria).Include(p => p.Autor)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToList();
            return View(posts);
        }

        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = _db.Posts.Include(p => p.Categoria).Include(p => p.Autor)
                .FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        public ActionResult Create()
        {
            ViewBag.CategoriaId = new SelectList(_db.Categories.OrderBy(c => c.Nombre), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                post.AutorId = User.Identity.GetUserId();
                post.FechaPublicacion = System.DateTime.Now;
                _db.Posts.Add(post);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoriaId = new SelectList(_db.Categories.OrderBy(c => c.Nombre), "Id", "Nombre", post.CategoriaId);
            return View(post);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = _db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            if (!User.IsInRole("Admin") && post.AutorId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            ViewBag.CategoriaId = new SelectList(_db.Categories.OrderBy(c => c.Nombre), "Id", "Nombre", post.CategoriaId);
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Post post)
        {
            if (!User.IsInRole("Admin") && post.AutorId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                post.FechaPublicacion = System.DateTime.Now;
                _db.Entry(post).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoriaId = new SelectList(_db.Categories.OrderBy(c => c.Nombre), "Id", "Nombre", post.CategoriaId);
            return View(post);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = _db.Posts.Include(p => p.Categoria).Include(p => p.Autor).FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var post = _db.Posts.Find(id);
            if (post != null)
            {
                _db.Posts.Remove(post);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
