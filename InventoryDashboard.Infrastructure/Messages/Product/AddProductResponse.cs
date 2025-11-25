namespace InventoryDashboard.Infrastructure.Messages.Product
{
    public class AddProductResponse
    {
        public string ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public string CategoryId { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string SKU { get; set; }

        public decimal? ConvertedPrice { get; set; }

        public string CurrencyCode { get; set; } = string.Empty;
    }
}