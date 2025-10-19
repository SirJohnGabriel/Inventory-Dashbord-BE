namespace InventoryDashboard.Infrastructure.Models.Products
{
    using System;
    using System.Runtime.Serialization;
    using InventoryDashboard.Infrastructure.Entities.Products;

    public class GetProductModel
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public Category Category { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public int StockQuantity { get; set; }

        [DataMember]
        public string SKU { get; set; }

        [DataMember]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DataMember]
        public DateTime UpdatedAt { get; set; }

        [DataMember]
        public Guid UpdatedBy { get; set; }

        [DataMember]
        public Guid CreatedBy { get; set; }

        [DataMember]
        public decimal? ConvertedPrice { get; set; }

        [DataMember]
        public string CurrencyCode { get; set; }
    }
}
