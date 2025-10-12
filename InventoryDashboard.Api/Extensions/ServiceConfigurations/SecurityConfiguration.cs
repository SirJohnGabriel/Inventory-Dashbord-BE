namespace InventoryDashboard.Api.Extensions.ServiceConfigurations
{
    using Microsoft.AspNetCore.Builder;

    public static class SecurityConfiguration
    {
        public static void UseSecurityPolicies(this WebApplication app)
        {
            app.UseHsts();
            app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

                context.Response.Headers.Append("X-Frame-Options", "DENY");

                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

                context.Response.Headers.Append("Referrer-Policy", "no-referrer");

                var cspValue = "default-src 'self'; " +
                              "style-src 'self' 'unsafe-inline'; " +
                              "font-src 'self'; " +
                              "img-src 'self'; " +
                              "script-src 'self'; " +
                              "object-src 'none'; " +
                              "form-action 'self'; " +
                              "frame-ancestors 'none'; " +
                              "block-all-mixed-content";
                context.Response.Headers.Append("Content-Security-Policy", cspValue);

                await next();
            });
        }
    }
}