namespace InventoryDashboard.Services.Identity.Extensions.Models
{
    using InventoryDashboard.Infrastructure.Entities.Identity;
    using InventoryDashboard.Infrastructure.Messages.Identity;

    public static class UserExtensions
    {
        public static User AsUser(this SignUpRequest request)
        {
            var result = new User
            {
                UserName = request.UserName,
                Email = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            return result;
        }
    }
}
