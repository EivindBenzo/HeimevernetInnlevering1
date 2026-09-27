using Kart.Models;
using Microsoft.EntityFrameworkCore;

namespace Kart.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ressurs> Ressurser { get; set; }
    }
}