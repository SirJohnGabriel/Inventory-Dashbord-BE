namespace InventoryDashboard.Services.Identity.Strategies.TokenValidator
{
    using InventoryDashboard.Services.Identity.Models;

    public class NullValidatorStrategy : ITokenValidatorStrategy
    {
        public TokenPayload ValidateToken(string idToken)
        {
            return new TokenPayload();
        }
    }
}
