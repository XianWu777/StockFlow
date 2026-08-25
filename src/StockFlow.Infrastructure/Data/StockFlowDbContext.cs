using Microsoft.EntityFrameworkCore;
using StockFlow.Infrastructure.Entity;

namespace StockFlow.Infrastructure.Data
{
    public class StockFlowDbContext : DbContext
    {
        public StockFlowDbContext(DbContextOptions<StockFlowDbContext> options)
                : base(options)
        {

        }

        public DbSet<Product> Products => Set<Product>();
    }
}