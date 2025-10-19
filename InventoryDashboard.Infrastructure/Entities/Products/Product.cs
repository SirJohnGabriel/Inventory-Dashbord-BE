namespace InventoryDashboard.Infrastructure.Entities.Products
{
    using System;

    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Category Category { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string SKU { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        public Guid UpdatedBy { get; set; }

        public Guid CreatedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
