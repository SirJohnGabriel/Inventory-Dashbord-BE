namespace InventoryDashboard.Api.Messages.Product
{
    public class ProductDataV2 : ProductData
    {
        public ProductDataV2(string id, string name, string description, string categoryId, decimal price, int stockQuantity, string sku, bool isDeleted)
            : base(id, name, description, categoryId, price, stockQuantity, sku, isDeleted)
        {
        }

        public decimal TaxAmount { get; set; }

        public decimal PriceWithTax { get; set; }

        public decimal? ConvertedPrice { get; set; }

        public string CurrencyCode { get; set; }
    }
}
