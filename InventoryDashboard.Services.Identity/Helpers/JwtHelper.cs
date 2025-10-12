namespace InventoryDashboard.Services.Identity.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using InventoryDashboard.Infrastructure.Enums.Identity;
    using InventoryDashboard.Infrastructure.Settings;
    using InventoryDashboard.Services.Identity.Models;
    using InventoryDashboard.Services.Identity.Strategies.TokenValidator;
    using Microsoft.IdentityModel.Tokens;

    public class JwtHelper
    {
        private readonly JwtSettings settings;
        private readonly IDictionary<string, string> configs;

        public JwtHelper()
        {
        }

        public JwtHelper(JwtSettings settings, IDictionary<string, string> configs)
        {
            this.settings = settings;
            this.configs = configs;
        }

        public virtual TokenPayload ValidateToken(string idToken, string provider)
        {
            var payload = new TokenPayload();

            if (Enum.TryParse(provider, out Provider providerEnum))
            {
                ITokenValidatorStrategy validator;

                switch (providerEnum)
                {
                    case Provider.MICROSOFT:
                        validator = new MicrosoftTokenValidatorStrategy(this.configs["MicrosoftClientId"], this.configs["MicrosoftAuthority"]);
                        break;
                    case Provider.GOOGLE:
                        validator = new GoogleTokenValidatorStrategy(this.configs["GoogleClientId"]);
                        break;
                    default:
                        validator = new NullValidatorStrategy();
                        break;
                }

                payload = validator.ValidateToken(idToken);
            }

            return payload;
        }

        public virtual string GenerateToken(IEnumerable<Claim> claims, bool overrideExpiry = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(overrideExpiry ? this.settings.RememberMeDuration : this.settings.ExpiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = this.configs["Issuer"],
                Audience = this.configs["Audience"],
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = tokenHandler.WriteToken(token);
            return result;
        }
    }
}