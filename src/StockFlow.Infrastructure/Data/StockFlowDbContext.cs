using Microsoft.EntityFrameworkCore;
using StockFlow.Infrastructure.Inventories;
using StockFlow.Infrastructure.Products;
using StockFlow.Infrastructure.Users;

namespace StockFlow.Infrastructure.Data;

public class StockFlowDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<User> Users => Set<User>();

    public StockFlowDbContext(DbContextOptions<StockFlowDbContext> options)
            : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.Property(x => x.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(x => x.UserName)
                .IsUnique();
        });

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

            entity.HasIndex(x => new
            {
                x.ProductId,
                x.CreatedAt
            });

            entity.Property(x => x.Type)
                .IsRequired();

            entity.HasIndex(x => x.ProductId);
        });
    }
}