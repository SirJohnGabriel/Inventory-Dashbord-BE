namespace InventoryDashboard.Services.Product.Data
{
    using Microsoft.EntityFrameworkCore;
    using InventoryDashboard.Infrastructure.Entities.Products;
    using InventoryDashboard.Infrastructure.Entities.Identity;

    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(250).IsRequired();
                entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(250);

                entity.Property(e => e.Price).HasColumnName("price");
                entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
                entity.Property(e => e.SKU).HasColumnName("sku").HasMaxLength(100).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                // Relationship: Product -> Category
                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey("category_id")
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();
                
                entity.HasIndex(e => e.SKU).IsUnique();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(250);

                // Seed initial categories
                entity.HasData(
                    new Category
                    {
                        Id = Guid.Parse("eb360ca0-ee0c-4be6-bc9b-7cb45644ae53"),
                        Name = "Electronics",
                        Description = "Electronic devices and gadgets."
                    },
                    new Category
                    {
                        Id = Guid.Parse("00b08252-2abd-46a5-a696-05d3d0609349"),
                        Name = "Office Supplies",
                        Description = "Office supplies and stationery."
                    },
                    new Category
                    {
                        Id = Guid.Parse("2503ae27-3b60-43da-9a53-943f7cde5295"),
                        Name = "Books",
                        Description = "Printed and electronic books."
                    }
                );
            });
        }
    }
}