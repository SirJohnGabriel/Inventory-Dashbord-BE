namespace InventoryDashboard.Infrastructure.Entities.Identity
{
    using System;
    using InventoryDashboard.Infrastructure.Enums.Identity;
    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser<Guid>
    {
        //// All custom user fields should go here.

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Active;

        public string Timezone { get; set; } = "UTC";

        public string TwoFactorSecret { get; set; }

        public string SsoProvider { get; set; }

        public string SsoSubjectId { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int RevisionNo { get; set; } = 1;
    }
}