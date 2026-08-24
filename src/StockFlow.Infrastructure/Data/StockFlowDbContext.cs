using Microsoft.EntityFrameworkCore;

namespace StockFlow.Infrastructure.Entity
{
    public class StockFlowDbContext : DbContext
    {
        public StockFlowDbContext(DbContextOptions<StockFlowDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products => Set<Product>();
    }
}