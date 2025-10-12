namespace InventoryDashboard.Infrastructure.Messages.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class AddToRoleRequest(string userId, string role)
    {
        [Required]
        public string Role { get; set; } = role;

        [Required]
        public string UserId { get; set; } = userId;
    }
}
