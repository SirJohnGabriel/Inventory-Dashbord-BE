namespace InventoryDashboard.Api.Messages.Product
{
    public class ProductData
    {
        public ProductData(string id, string name, string description, string categoryId, decimal price, int stockQuantity, string sku, bool isDeleted)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.CategoryId = categoryId;
            this.Price = price;
            this.StockQuantity = stockQuantity;
            this.SKU = sku;
            this.IsDeleted = isDeleted;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CategoryId { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string SKU { get; set; }

        public bool IsDeleted { get; set; }
    }
}