using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BlogNoticiasV2.Models;

namespace BlogNoticiasV2.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var posts = _db.Posts.Include(p => p.Autor).Include(p => p.Categoria)
                .OrderByDescending(p => p.FechaPublicacion)
                .Take(5)
                .ToList();
            return View(posts);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Blog de noticias construido con ASP.NET MVC 5.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contacta con el equipo de BlogNoticiasV2.";
            return View();
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
