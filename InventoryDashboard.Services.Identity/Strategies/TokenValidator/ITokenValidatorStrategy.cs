namespace InventoryDashboard.Services.Identity.Strategies.TokenValidator
{
    using InventoryDashboard.Services.Identity.Models;

    public interface ITokenValidatorStrategy
    {
        TokenPayload ValidateToken(string idToken);
    }
}
