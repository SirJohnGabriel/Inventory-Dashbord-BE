namespace InventoryDashboard.Services.Identity.Extensions
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using InventoryDashboard.Services.Identity.Models;

    internal static class TokenPayloadExtensions
    {
        public static IEnumerable<Claim> AsClaims(this TokenPayload payload, string userId = "")
        {
            var result = new List<Claim>
            {
                new Claim(ClaimTypes.Name, payload.Email),
                new Claim(ClaimTypes.Email, payload.Email),
            };

            if (!string.IsNullOrEmpty(payload.FirstName))
            {
                result.Add(new Claim(ClaimTypes.GivenName, payload.FirstName));
            }

            if (!string.IsNullOrEmpty(payload.LastName))
            {
                result.Add(new Claim(ClaimTypes.Surname, payload.LastName));
            }

            if (!string.IsNullOrEmpty(userId))
            {
                result.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            }

            return result;
        }
    }
}