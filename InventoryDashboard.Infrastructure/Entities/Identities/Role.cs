namespace InventoryDashboard.Infrastructure.Entities.Identity
{
    using System;
    using Microsoft.AspNetCore.Identity;

    public class Role : IdentityRole<Guid>
    {
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
