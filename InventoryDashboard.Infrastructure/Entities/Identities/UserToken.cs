namespace InventoryDashboard.Infrastructure.Entities.Identity
{
    using System;
    using Microsoft.AspNetCore.Identity;

    public class UserToken : IdentityUserToken<Guid>
    {
    }
}