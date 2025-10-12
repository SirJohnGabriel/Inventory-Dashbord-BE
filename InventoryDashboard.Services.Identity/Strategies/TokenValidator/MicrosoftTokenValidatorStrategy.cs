namespace InventoryDashboard.Services.Identity.Strategies.TokenValidator
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using InventoryDashboard.Services.Identity.Models;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;

    public class MicrosoftTokenValidatorStrategy : ITokenValidatorStrategy
    {
        private readonly string clientId;
        private readonly string authority;

        public MicrosoftTokenValidatorStrategy(string clientId, string authority)
        {
            this.clientId = clientId;
            this.authority = authority;
        }

        public TokenPayload ValidateToken(string idToken)
        {
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{this.authority}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            var config = configurationManager.GetConfigurationAsync().Result;

            var oidconfig = config;

            var validationParameters = new TokenValidationParameters()
            {
                ValidIssuer = oidconfig?.Issuer,
                ValidAudience = this.clientId,
                IssuerSigningKeys = config.SigningKeys,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ClockSkew = TimeSpan.FromSeconds(5),
                ValidateLifetime = true,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(idToken);
            var claimsPrincipal = tokenHandler.ValidateToken(idToken, validationParameters, out var validatedToken);
            var claims = claimsPrincipal.Claims.ToArray();
            var names = claims.FirstOrDefault(claim => claim.Type == "name")?.Value.Split(' ');

            var result = new TokenPayload { Subject = claims[10].Value, Email = claims[8].Value, FirstName = names[0], LastName = names[names.Length - 1] };
            return result;
        }
    }
}