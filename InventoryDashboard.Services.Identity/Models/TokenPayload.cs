namespace InventoryDashboard.Services.Identity.Models
{
    public class TokenPayload
    {
        public string Email { get; set; }

        public string Subject { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}