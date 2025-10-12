namespace InventoryDashboard.Api.Extensions.Injection
{
    using System.Text;
    using FluentValidation;
    using InventoryDashboard.Infrastructure.Entities.Identity;
    using InventoryDashboard.Infrastructure.Enums.Identity;
    using InventoryDashboard.Infrastructure.Messages.Identity;
    using InventoryDashboard.Infrastructure.Services.Interfaces;
    using InventoryDashboard.Infrastructure.Settings;
    using InventoryDashboard.Services.Identity;
    using InventoryDashboard.Services.Identity.Data;
    using InventoryDashboard.Services.Identity.Helpers;
    using InventoryDashboard.Services.Identity.Validators;
    using InventoryDashboard.Services.Identity.Validators.SignIn;
    using InventoryDashboard.Services.Identity.Validators.SignUp;
    using InventoryDashboard.Services.Identity.Workflows;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    public static class IdentityInjection
    {
        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("PostgreSQL")
                ?? throw new InvalidOperationException("PostgreSQL connection string is required");

            // Use simple connection string approach - no complex enum mapping needed
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString).EnableSensitiveDataLogging().LogTo(Console.WriteLine, LogLevel.Information));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;

                // options.SignIn.RequireConfirmedEmail = Convert.ToBoolean(config.GetSection("Identity")["RequireEmailVerification"]);
            });

            var jwtConfig = config.GetSection("Jwt");
            var secret = jwtConfig["Secret"] ?? throw new InvalidOperationException("JWT Secret is required");
            var key = Encoding.ASCII.GetBytes(secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidIssuer = jwtConfig["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is required"),
                    ValidAudience = jwtConfig["Audience"] ?? throw new InvalidOperationException("JWT Audience is required"),
                };
            });

            var expiresInMinutes = int.TryParse(jwtConfig["ExpiresInMinutes"], out var expiresInMinutesResult) ? expiresInMinutesResult : 60; // Default to 60 minutes
            var rememberMeDuration = int.TryParse(jwtConfig["RememberMeDuration"], out var rememberMeDurationResult) ? rememberMeDurationResult : 10080; // Default to 7 days
            var jwtSettings = new JwtSettings(secret, expiresInMinutes, rememberMeDuration);
            var jwtConfigs = new Dictionary<string, string>()
            {
                { "GoogleClientId", config.GetSection("OAuth:Google")["ClientId"] ?? string.Empty },
                { "MicrosoftClientId", config.GetSection("OAuth:Microsoft")["ClientId"] ?? string.Empty },
                { "MicrosoftAuthority", config.GetSection("OAuth:Microsoft")["Authority"] ?? string.Empty },
                { "Issuer", jwtConfig["Issuer"] ?? string.Empty },
                { "Audience", jwtConfig["Audience"] ?? string.Empty },
            };
            var jwtHelper = new JwtHelper(jwtSettings, jwtConfigs);
            services.AddSingleton(jwtSettings);
            services.AddScoped(typeof(JwtHelper), instance => jwtHelper);
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddScoped(typeof(IdentityServiceWorkflows));

            //// Inject validators
            services.AddScoped<IValidator<SignUpRequest>, SignUpValidator>();
            services.AddScoped<IValidator<SignInRequest>, SignInValidator>();
            services.AddScoped(typeof(IdentityServiceValidators));
        }
    }
}
