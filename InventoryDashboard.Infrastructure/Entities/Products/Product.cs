namespace InventoryDashboard.Infrastructure.Entities.Products
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Products")]
    public class Product
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("category_id")]
        public Category Category { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("stock_quantity")]
        public int StockQuantity { get; set; }

        [Column("sku")]
        public string SKU { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("updated_by")]
        public Guid UpdatedBy { get; set; }

        [Column("created_by")]
        public Guid CreatedBy { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
    }
}
