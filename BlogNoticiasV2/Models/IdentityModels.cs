using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BlogNoticiasV2.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Nombre")]
        public string DisplayName { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            if (!string.IsNullOrWhiteSpace(DisplayName))
            {
                userIdentity.AddClaim(new Claim("DisplayName", DisplayName));
            }
            return userIdentity;
        }
    }

    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Contenido")]
        public string Contenido { get; set; }

        [Display(Name = "Fecha de publicación")]
        public System.DateTime FechaPublicacion { get; set; }

        [Required]
        public string AutorId { get; set; }

        [Display(Name = "Autor")]
        public virtual ApplicationUser Autor { get; set; }

        [Required]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        public virtual Category Categoria { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
