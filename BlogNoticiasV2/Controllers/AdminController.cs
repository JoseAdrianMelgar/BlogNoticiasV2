using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BlogNoticiasV2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace BlogNoticiasV2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public RoleManager<IdentityRole> RoleManager
        {
            get => _roleManager ?? HttpContext.GetOwinContext().Get<RoleManager<IdentityRole>>();
            private set => _roleManager = value;
        }

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ActionResult Index()
        {
            var model = UserManager.Users.ToList().Select(u => new AdminUserViewModel
            {
                UserId = u.Id,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Roles = UserManager.GetRoles(u.Id)
            }).ToList();

            ViewBag.Roles = RoleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                TempData["Error"] = "Usuario o rol inválido.";
                return RedirectToAction("Index");
            }

            if (!await RoleManager.RoleExistsAsync(roleName))
            {
                TempData["Error"] = "El rol especificado no existe.";
                return RedirectToAction("Index");
            }

            var userRoles = await UserManager.GetRolesAsync(userId);
            await UserManager.RemoveFromRolesAsync(userId, userRoles.ToArray());
            await UserManager.AddToRoleAsync(userId, roleName);
            TempData["Message"] = "Rol asignado correctamente.";
            return RedirectToAction("Index");
        }
    }

    public class AdminUserViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public System.Collections.Generic.IList<string> Roles { get; set; }
    }
}
