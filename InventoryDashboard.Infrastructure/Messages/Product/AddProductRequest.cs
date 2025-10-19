namespace InventoryDashboard.Infrastructure.Messages.Product
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AddProductRequest
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        [Required]
        [StringLength(100)]
        public string SKU { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public Guid UpdatedBy { get; set; }
    }
}