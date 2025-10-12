namespace InventoryDashboard.Services.Identity.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using InventoryDashboard.Infrastructure.Constants.Errors;
    using InventoryDashboard.Infrastructure.Entities.Identity;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Identity;
    using Microsoft.AspNetCore.Identity;

    internal static class RoleManagerExtensions
    {
        public static async Task<Response> AddToRoleAsync(this RoleManager<Role> roleManager, UserManager<User> userManager, AddToRoleRequest request)
        {
            var result = new Response();

            var user = await userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                result.SetError(IdentityServiceErrorCodes.UserNotFound);
                return result;
            }

            Claim claim;
            IEnumerable<Claim> claims;
            Role role;

            if (!await roleManager.RoleExistsAsync(request.Role))
            {
                role = new Role { Id = Guid.NewGuid(), Name = request.Role };
                await roleManager.CreateAsync(role);
                claim = new Claim(ClaimTypes.Role, request.Role);
                await roleManager.AddClaimAsync(role, claim);
                claims = new Claim[] { claim };
            }
            else
            {
                role = await roleManager.FindByNameAsync(request.Role);
                claims = await roleManager.GetClaimsAsync(role);
            }

            await userManager.AddToRoleAsync(user, request.Role);
            await userManager.AddClaimsAsync(user, claims);

            return result;
        }
    }
}
