namespace InventoryDashboard.Infrastructure.Messages.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordRequest(string email, string newPassword, string token)
    {
        [Required]
        public string Email { get; set; } = email;

        [Required]
        public string NewPassword { get; set; } = newPassword;

        [Required]
        public string Token { get; set; } = token;
    }
}
