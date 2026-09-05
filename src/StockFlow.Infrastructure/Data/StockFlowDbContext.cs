using Microsoft.EntityFrameworkCore;
using StockFlow.Infrastructure.Inventories;
using StockFlow.Infrastructure.Products;

namespace StockFlow.Infrastructure.Data;

public class StockFlowDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

    public StockFlowDbContext(DbContextOptions<StockFlowDbContext> options)
            : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Version)
                .IsConcurrencyToken();

            entity.HasOne(x => x.Product)
                .WithOne()
                .HasForeignKey<Inventory>(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.ProductId)
                .IsUnique();
        });

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type)
                .IsRequired();

            entity.HasIndex(x => x.ProductId);
        });
    }
}