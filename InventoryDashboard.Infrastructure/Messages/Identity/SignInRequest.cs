namespace InventoryDashboard.Infrastructure.Messages.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class SignInRequest(string userName, string password)
    {
        [Required]
        public string UserName { get; set; } = userName;

        [Required]
        public string Password { get; set; } = password;
    }
}
