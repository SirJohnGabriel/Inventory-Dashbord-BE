namespace InventoryDashboard.Api.Extensions.ServiceConfigurations
{
    using Microsoft.AspNetCore.Builder;
    using Scalar.AspNetCore;

    public static class OpenApiConfiguration
    {
        public static void ConfigureOpenApi(this WebApplication app)
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("InventoryDashboard API")
                    .WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                    .WithDarkMode();
            });
        }
    }
}