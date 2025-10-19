namespace InventoryDashboard.Services.Product.Data.Seeders
{
    using System;
    using System.Threading.Tasks;
    using InventoryDashboard.Infrastructure.Entities.Products;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class CategorySeedData
    {
        public static async Task EnsureSeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

            if (!await db.Categories.AnyAsync())
            {
                var categories = new[]
                {
                    new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Electronics", Description = "Electronic devices and gadgets." },
                    new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Office Supplies", Description = "Office supplies and stationery." },
                    new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Books", Description = "Printed and electronic books" },
                };

                await db.Categories.AddRangeAsync(categories);
                await db.SaveChangesAsync();
            }
        }
    }
}