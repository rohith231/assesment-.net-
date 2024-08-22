using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;

namespace SampleAPI.Repositories
{
    public class SampleApiDbContext : DbContext
    {
        public SampleApiDbContext(DbContextOptions<SampleApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
    }
}
