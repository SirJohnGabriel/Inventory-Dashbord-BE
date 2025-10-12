namespace InventoryDashboard.Api.Common.Models
{
    public class UserWebModel
    {
        public UserWebModel(Guid userId, string firstName, string lastName, string email)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public UserWebModel()
        {
        }

        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}