namespace InventoryDashboard.Infrastructure.Messages.Identity
{
    using System.ComponentModel.DataAnnotations;
    using InventoryDashboard.Infrastructure.Enums.Identity;

    public class SignUpRequest(string userName, string firstName, string lastName, string password, string confirmPassword, CustomRole defaultRole)
    {
        [Required]
        public string UserName { get; set; } = userName;

        public string FirstName { get; set; } = firstName;

        public string LastName { get; set; } = lastName;

        [Required]
        public string Password { get; set; } = password;

        [Required]
        public string ConfirmPassword { get; set; } = confirmPassword;

        public CustomRole DefaultRole { get; set; } = defaultRole;
    }
}
