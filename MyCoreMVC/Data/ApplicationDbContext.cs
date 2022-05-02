using Microsoft.EntityFrameworkCore;
using MyCoreMVC_20220327.Models;

namespace MyCoreMVC_20220327.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
    }
}
