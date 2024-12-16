using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Entities;

namespace OrderApi.Infrastructure.Data
{
    public class OrdreDbContext : DbContext
    {
        public OrdreDbContext(DbContextOptions<OrdreDbContext> options) : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
    }
}
