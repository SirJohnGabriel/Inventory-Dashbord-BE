namespace InventoryDashboard.Infrastructure.Entities.Products
{
    using System;

    public class Category
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
