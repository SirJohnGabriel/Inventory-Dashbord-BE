namespace InventoryDashboard.Services.Identity.Strategies.TokenValidator
{
    using System.Collections.Generic;
    using Google.Apis.Auth;
    using InventoryDashboard.Services.Identity.Models;

    public class GoogleTokenValidatorStrategy : ITokenValidatorStrategy
    {
        private readonly string clientId;

        public GoogleTokenValidatorStrategy(string clientId)
        {
            this.clientId = clientId;
        }

        public TokenPayload ValidateToken(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { this.clientId },
            };

            var payload = GoogleJsonWebSignature.ValidateAsync(idToken, settings).Result;

            var result = new TokenPayload { Subject = payload.Subject, Email = payload.Email, FirstName = payload.GivenName, LastName = payload.FamilyName };
            return result;
        }
    }
}
