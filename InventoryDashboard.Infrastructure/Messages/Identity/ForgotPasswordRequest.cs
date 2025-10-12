namespace InventoryDashboard.Infrastructure.Messages.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class ForgotPasswordRequest(string email)
    {
        [Required]
        public string Email { get; set; } = email;
    }
}
