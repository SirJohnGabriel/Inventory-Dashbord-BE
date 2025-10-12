namespace InventoryDashboard.Infrastructure.Entities.Identity
{
    using System;
    using Microsoft.AspNetCore.Identity;

    public class UserRole : IdentityUserRole<Guid>
    {
        // Keep Id as a unique identifier but not the primary key
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;

        public DateTime? EffectiveTo { get; set; }

        public Guid? AssignedBy { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User AssignedByUser { get; set; }
    }
}
