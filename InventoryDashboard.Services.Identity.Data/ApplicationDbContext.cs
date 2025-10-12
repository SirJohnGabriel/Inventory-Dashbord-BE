namespace InventoryDashboard.Services.Identity.Data
{
    using System;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using InventoryDashboard.Infrastructure.Enums.Identity;
    using InventoryDashboard.Infrastructure.Entities.Identity;

    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public new DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Enum mapping is handled by the data source builder in IdentityInjection
            // builder.HasPostgresEnum<UserStatus>("user_status");

            //// Customize the ASP.NET Core Identity model and override the defaults if needed.
            //// For example, you can rename the ASP.NET Core Identity table names and more.
            //// Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "users");
                // Map the primary key
                entity.Property(e => e.Id).HasColumnName("id");
                // Map additional properties
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.Status)
                    .HasColumnName("user_status")
                    .HasConversion<string>();
                entity.Property(e => e.Timezone).HasColumnName("timezone");
                entity.Property(e => e.TwoFactorSecret).HasColumnName("two_factor_secret");
                entity.Property(e => e.SsoProvider).HasColumnName("sso_provider");
                entity.Property(e => e.SsoSubjectId).HasColumnName("sso_subject_id");
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
                entity.Property(e => e.RevisionNo).HasColumnName("revision_no");
                entity.Property(e => e.UserName).HasColumnName("user_name");
                entity.Property(e => e.NormalizedUserName).HasColumnName("normalized_user_name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.NormalizedEmail).HasColumnName("normalized_email");
                entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
                entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
                entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
                entity.Property(e => e.TwoFactorEnabled).HasColumnName("two_factor_enabled");
                entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
                entity.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
                entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
            });

            builder.Entity<Role>(entity =>
            {
                entity.ToTable(name: "roles");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.NormalizedName).HasColumnName("normalized_name");
                entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.ToTable(name: "user_roles");
                // Use composite key as expected by Identity
                entity.HasKey(e => new { e.UserId, e.RoleId });
                // Map the Id column but it's not the primary key
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.EffectiveFrom)
                    .HasColumnName("effective_from")
                    .HasDefaultValueSql("now()");
                entity.Property(e => e.EffectiveTo).HasColumnName("effective_to");
                entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
                entity.Property(e => e.AssignedAt)
                    .HasColumnName("assigned_at")
                    .HasDefaultValueSql("now()");

                // Add unique constraint for the Id column
                entity.HasIndex(e => e.Id)
                    .IsUnique()
                    .HasDatabaseName("uk_user_roles_id");

                entity.HasOne(e => e.AssignedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<UserClaim>(entity =>
            {
                entity.ToTable(name: "user_claims");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ClaimType).HasColumnName("claim_type");
                entity.Property(e => e.ClaimValue).HasColumnName("claim_value");
            });

            builder.Entity<RoleClaim>(entity =>
            {
                entity.ToTable(name: "role_claims");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.ClaimType).HasColumnName("claim_type");
                entity.Property(e => e.ClaimValue).HasColumnName("claim_value");
            });

            builder.Entity<UserLogin>(entity =>
            {
                entity.ToTable(name: "user_logins");
                entity.Property(e => e.LoginProvider).HasColumnName("login_provider");
                entity.Property(e => e.ProviderKey).HasColumnName("provider_key");
                entity.Property(e => e.ProviderDisplayName).HasColumnName("provider_display_name");
                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            builder.Entity<UserToken>(entity =>
            {
                entity.ToTable(name: "user_tokens");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.LoginProvider).HasColumnName("login_provider");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Value).HasColumnName("value");
            });
        }
    }
}
