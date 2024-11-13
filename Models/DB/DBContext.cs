using Microsoft.EntityFrameworkCore;

namespace Veeb.Models.DB
{
    public class DBContext : DbContext
    {
        public DbSet<Toode> Tooded { get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }
    }
}
