namespace InventoryDashboard.Infrastructure.Messages.Identity
{
    using System.ComponentModel.DataAnnotations;
    using InventoryDashboard.Infrastructure.Enums.Identity;

    public class SignUpRequest(string userName, string password, string firstName, string lastName, string confirmPassword, CustomRole defaultRole)
    {
        [Required]
        public string UserName { get; set; } = userName;

        [Required]
        public string Password { get; set; } = password;

        public string FirstName { get; set; } = firstName;

        public string LastName { get; set; } = lastName;

        [Required]
        public string ConfirmPassword { get; set; } = confirmPassword;

        public CustomRole DefaultRole { get; set; } = defaultRole;
    }
}
