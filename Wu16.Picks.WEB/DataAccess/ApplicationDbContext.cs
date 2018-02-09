using Microsoft.EntityFrameworkCore;
using Wu16.Picks.WEB.Models.Domain;

namespace Wu16.Picks.WEB.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}