namespace InventoryDashboard.Infrastructure.Settings
{
    public class JwtSettings
    {
        public JwtSettings(string secret, int expiresInMinutes, int rememberMeDuration)
        {
            this.Secret = secret;
            this.ExpiresInMinutes = expiresInMinutes;
            this.RememberMeDuration = rememberMeDuration;
        }

        public string Secret { get; private set; }

        public int ExpiresInMinutes { get; private set; }

        public int RememberMeDuration { get; private set; }
    }
}