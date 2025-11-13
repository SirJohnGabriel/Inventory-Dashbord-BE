namespace InventoryDashboard.Services.Identity.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;

    public static class MockHelpers
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>()
            where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
            return mgr;
        }

        public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(IRoleStore<TRole>? store = null)
            where TRole : class
        {
            store ??= new Mock<IRoleStore<TRole>>().Object;
            var roles = new List<IRoleValidator<TRole>>
            {
                new RoleValidator<TRole>(),
            };
            return new Mock<RoleManager<TRole>>(
                store,
                roles,
                MockLookupNormalizer(),
                new IdentityErrorDescriber(),
                null!);
        }

        public static Mock<SignInManager<TUser>> MockSignInManager<TUser>()
            where TUser : class
        {
            var userManager = MockUserManager<TUser>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();

            _ = new SignInManager<TUser>(userManager.Object, contextAccessor.Object, userClaimsPrincipalFactory.Object, options.Object, null, schemes.Object);
            return new Mock<SignInManager<TUser>>(userManager.Object, contextAccessor.Object, userClaimsPrincipalFactory.Object, options.Object, null, schemes.Object);
        }

        public static UserManager<TUser> TestUserManager<TUser>(IUserStore<TUser>? store = null)
            where TUser : class
        {
            store ??= new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;
            options.Setup(o => o.Value).Returns(idOptions);
            var userValidators = new List<IUserValidator<TUser>>();
            var validator = new Mock<IUserValidator<TUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>>
            {
                new PasswordValidator<TUser>(),
            };
            var userManager = new UserManager<TUser>(
                store,
                options.Object,
                new PasswordHasher<TUser>(),
                userValidators,
                pwdValidators,
                MockLookupNormalizer(),
                new IdentityErrorDescriber(),
                null!,
                new Mock<ILogger<UserManager<TUser>>>().Object);
            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
            return userManager;
        }

        public static RoleManager<TRole> TestRoleManager<TRole>(IRoleStore<TRole>? store = null)
            where TRole : class
        {
            store ??= new Mock<IRoleStore<TRole>>().Object;
            var roles = new List<IRoleValidator<TRole>>
            {
                new RoleValidator<TRole>(),
            };
            return new RoleManager<TRole>(
                store,
                roles,
                MockLookupNormalizer(),
                new IdentityErrorDescriber(),
                null!);
        }

        public static ILookupNormalizer MockLookupNormalizer()
        {
            var normalizerFunc = new Func<string?, string>(i =>
            {
                return i == null ? string.Empty : Convert.ToBase64String(Encoding.UTF8.GetBytes(i)).ToUpperInvariant();
            });
            var lookupNormalizer = new Mock<ILookupNormalizer>();
            lookupNormalizer.Setup(i => i.NormalizeName(It.IsAny<string>())).Returns(normalizerFunc);
            lookupNormalizer.Setup(i => i.NormalizeEmail(It.IsAny<string>())).Returns(normalizerFunc);
            return lookupNormalizer.Object;
        }
    }
}
