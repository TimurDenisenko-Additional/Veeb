using Microsoft.EntityFrameworkCore;

namespace Veeb.Models.DB
{
    public class DBContext : DbContext
    {
        //add-migration 
        //update-database
        public DbSet<Toode> Tooded { get; set; }
        public DbSet<Kasutaja> Kasutajad { get; set; }
        public DbSet<Order> Ordered {  get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }
    }
}
