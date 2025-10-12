namespace InventoryDashboard.Api.Extensions.ServiceConfigurations
{
    using InventoryDashboard.Api.Constants;
    using Microsoft.AspNetCore.Mvc;

    public static class ApiBehaviorConfiguration
    {
        public static void ConfigureApiBehavior(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var error = new Dictionary<string, string>();

                    var errors = context.ModelState.Keys
                        .SelectMany(key => context.ModelState[key].Errors.Select(x => new { key, x.ErrorMessage }))
                        .Select(a => a.ErrorMessage);

                    var response = new { ErrorCode = ValidationErrors.ModelState, Message = string.Join(string.Empty, errors) };
                    return new BadRequestObjectResult(response);
                };
            });
        }
    }
}