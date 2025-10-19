namespace InventoryDashboard.Infrastructure.Messages.Product
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UpdateProductRequest
    {
        [Required]
        public Guid Id { get; set; }

        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string Description { get; set; } = string.Empty;

        public Guid? CategoryId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }

        [StringLength(100)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        public Guid UpdatedBy { get; set; }
    }
}