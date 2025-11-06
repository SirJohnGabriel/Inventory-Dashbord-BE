namespace InventoryDashboard.Services.Identity.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using InventoryDashboard.Infrastructure.Constants.Errors;
    using InventoryDashboard.Infrastructure.Entities.Identity;
    using InventoryDashboard.Infrastructure.Enums.Identity;
    using InventoryDashboard.Infrastructure.Exceptions;
    using InventoryDashboard.Infrastructure.Logging;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Identity;
    using InventoryDashboard.Infrastructure.Validations;
    using InventoryDashboard.Services.Identity.Helpers;
    using InventoryDashboard.Services.Identity.Tests.Helpers;
    using InventoryDashboard.Services.Identity.Workflows;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class IdentityServiceTests
    {
        private IdentityService identityService = null!;
        private Mock<UserManager<User>> mockUserManager = null!;
        private Mock<SignInManager<User>> mockSignInManager = null!;
        private Mock<RoleManager<Role>> mockRoleManager = null!;
        private Mock<JwtHelper> mockJwtHelper = null!;
        private Mock<ILogger> mockLogger = null!;
        private Mock<Validator> mockValidator = null!;
        private IdentityServiceWorkflows workflows = null!;

        [TestInitialize]
        public void Setup()
        {
            this.mockValidator = new Mock<Validator>();
            this.mockLogger = new Mock<ILogger>();
            this.mockJwtHelper = new Mock<JwtHelper>();
            this.mockRoleManager = MockHelpers.MockRoleManager<Role>();
            this.mockSignInManager = MockHelpers.MockSignInManager<User>();
            this.mockUserManager = MockHelpers.MockUserManager<User>();

            this.workflows = new IdentityServiceWorkflows(
                this.mockUserManager.Object,
                this.mockRoleManager.Object,
                this.mockJwtHelper.Object);

            this.identityService = new IdentityService(
                this.mockUserManager.Object,
                this.mockSignInManager.Object,
                this.mockRoleManager.Object,
                this.mockJwtHelper.Object,
                this.mockValidator.Object,
                this.workflows,
                this.mockLogger.Object);
        }

        #region SignInAsync Tests

        [TestMethod]
        public async Task SignInAsync_WithValidCredentials_ReturnsSuccessWithToken()
        {
            // Arrange
            var request = new SignInRequest("testuser", "Password123!");
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Status = UserStatus.Active,
            };
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };
            var expectedToken = "jwt-token-123";

            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockSignInManager
                .Setup(s => s.PasswordSignInAsync(request.UserName, request.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            this.mockUserManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync(user);
            this.mockUserManager.Setup(u => u.GetClaimsAsync(user)).ReturnsAsync(claims);
            this.mockJwtHelper.Setup(j => j.GenerateToken(It.IsAny<IEnumerable<Claim>>(), It.IsAny<bool>())).Returns(expectedToken);

            // Act
            var result = await this.identityService.SignInAsync(request);

            // Assert
            Assert.AreEqual(expectedToken, result.Data);
        }

        [TestMethod]
        public async Task SignInAsync_WithValidCredentials_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new SignInRequest("testuser", "Password123!");
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Status = UserStatus.Active,
            };
            var claims = new List<Claim>();

            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockSignInManager
                .Setup(s => s.PasswordSignInAsync(request.UserName, request.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            this.mockUserManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync(user);
            this.mockUserManager.Setup(u => u.GetClaimsAsync(user)).ReturnsAsync(claims);
            this.mockJwtHelper.Setup(j => j.GenerateToken(It.IsAny<IEnumerable<Claim>>(), It.IsAny<bool>())).Returns("token");

            // Act
            var result = await this.identityService.SignInAsync(request);

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorCode));
        }

        [TestMethod]
        public async Task SignInAsync_WithInvalidCredentials_ReturnsInvalidCredentialError()
        {
            // Arrange
            var request = new SignInRequest("testuser", "WrongPassword");
            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockSignInManager
                .Setup(s => s.PasswordSignInAsync(request.UserName, request.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await this.identityService.SignInAsync(request);

            // Assert
            Assert.AreEqual(IdentityServiceErrorCodes.InvalidCredential, result.ErrorCode);
        }

        [TestMethod]
        public async Task SignInAsync_WithSuspendedUser_ReturnsUserSuspendedError()
        {
            // Arrange
            var request = new SignInRequest("testuser", "Password123!");
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Status = UserStatus.Suspended,
            };

            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockSignInManager
                .Setup(s => s.PasswordSignInAsync(request.UserName, request.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            this.mockUserManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync(user);

            // Act
            var result = await this.identityService.SignInAsync(request);

            // Assert
            Assert.AreEqual(IdentityServiceErrorCodes.UserSuspended, result.ErrorCode);
        }

        [TestMethod]
        public async Task SignInAsync_WithDisabledUser_ReturnsUserDisabledError()
        {
            // Arrange
            var request = new SignInRequest("testuser", "Password123!");
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Status = UserStatus.Disabled,
            };

            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockSignInManager
                .Setup(s => s.PasswordSignInAsync(request.UserName, request.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            this.mockUserManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync(user);

            // Act
            var result = await this.identityService.SignInAsync(request);

            // Assert
            Assert.AreEqual(IdentityServiceErrorCodes.UserDisabled, result.ErrorCode);
        }

        [TestMethod]
        public async Task SignInAsync_UserWithFirstName_AddsGivenNameClaim()
        {
            // Arrange
            var request = new SignInRequest("testuser", "Password123!");
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                FirstName = "John",
                Status = UserStatus.Active,
            };
            var claims = new List<Claim>();
            List<Claim>? capturedClaims = null;

            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockSignInManager
                .Setup(s => s.PasswordSignInAsync(request.UserName, request.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            this.mockUserManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync(user);
            this.mockUserManager.Setup(u => u.GetClaimsAsync(user)).ReturnsAsync(claims);
            this.mockJwtHelper
                .Setup(j => j.GenerateToken(It.IsAny<IEnumerable<Claim>>(), It.IsAny<bool>()))
                .Returns((List<Claim> c, bool b) =>
                {
                    capturedClaims = c;
                    return "token";
                });

            // Act
            await this.identityService.SignInAsync(request);

            // Assert
            Assert.IsTrue(capturedClaims!.Any(c => c.Type == ClaimTypes.GivenName && c.Value == "John"));
        }

        [TestMethod]
        public async Task SignInAsync_UserWithLastName_AddsSurnameClaim()
        {
            // Arrange
            var request = new SignInRequest("testuser", "Password123!");
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                LastName = "Doe",
                Status = UserStatus.Active,
            };
            var claims = new List<Claim>();
            List<Claim>? capturedClaims = null;

            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockSignInManager
                .Setup(s => s.PasswordSignInAsync(request.UserName, request.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            this.mockUserManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync(user);
            this.mockUserManager.Setup(u => u.GetClaimsAsync(user)).ReturnsAsync(claims);
            this.mockJwtHelper
                .Setup(j => j.GenerateToken(It.IsAny<IEnumerable<Claim>>(), It.IsAny<bool>()))
                .Returns((List<Claim> c, bool b) =>
                {
                    capturedClaims = c;
                    return "token";
                });

            // Act
            await this.identityService.SignInAsync(request);

            // Assert
            Assert.IsTrue(capturedClaims!.Any(c => c.Type == ClaimTypes.Surname && c.Value == "Doe"));
        }

        [TestMethod]
        public async Task SignInAsync_WithValidationError_ReturnsValidationError()
        {
            // Arrange
            var request = new SignInRequest(string.Empty, string.Empty);
            this.mockValidator
                .Setup(v => v.ValidateAndThrow(request))
                .Throws(new ValidationException("Validation failed"));

            // Act
            var result = await this.identityService.SignInAsync(request);

            // Assert
            Assert.AreEqual(IdentityServiceErrorCodes.ValidationError, result.ErrorCode);
        }

        [TestMethod]
        [ExpectedException(typeof(IdentityServiceException))]
        public async Task SignInAsync_WithUnexpectedException_ThrowsIdentityServiceException()
        {
            // Arrange
            var request = new SignInRequest("testuser", "Password123!");
            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockSignInManager
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            await this.identityService.SignInAsync(request);
        }

        [TestMethod]
        public async Task SignInAsync_WithUnexpectedException_LogsException()
        {
            // Arrange
            var request = new SignInRequest("testuser", "Password123!");
            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockSignInManager
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            try
            {
                await this.identityService.SignInAsync(request);
            }
            catch
            {
                // Expected
            }

            // Assert
            this.mockLogger.Verify(l => l.WriteException(It.IsAny<IdentityServiceException>(), null), Times.Once);
        }

        #endregion

        #region SignUpAsync Tests

        [TestMethod]
        public async Task SignUpAsync_WithValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new SignUpRequest("testuser", "John", "Doe", "Password123!", "Password123!", CustomRole.User);
            var user = new User { Id = Guid.NewGuid(), UserName = "testuser" };

            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockUserManager
                .Setup(u => u.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            this.mockUserManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync(user);
            this.mockUserManager
                .Setup(u => u.AddClaimsAsync(user, It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await this.identityService.SignUpAsync(request);

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorCode));
        }

        [TestMethod]
        public async Task SignUpAsync_WithValidRequest_CreatesUser()
        {
            // Arrange
            var request = new SignUpRequest("testuser", "John", "Doe", "Password123!", "Password123!", CustomRole.User);
            var user = new User { Id = Guid.NewGuid(), UserName = "testuser" };

            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockUserManager
                .Setup(u => u.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            this.mockUserManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync(user);
            this.mockUserManager
                .Setup(u => u.AddClaimsAsync(user, It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await this.identityService.SignUpAsync(request);

            // Assert
            this.mockUserManager.Verify(u => u.CreateAsync(It.IsAny<User>(), request.Password), Times.Once);
        }

        [TestMethod]
        public async Task SignUpAsync_WithValidRequest_AddsNameIdentifierClaim()
        {
            // Arrange
            var request = new SignUpRequest("testuser", "John", "Doe", "Password123!", "Password123!", CustomRole.User);
            var user = new User { Id = Guid.NewGuid(), UserName = "testuser" };

            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockUserManager
                .Setup(u => u.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            this.mockUserManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync(user);
            this.mockUserManager
                .Setup(u => u.AddClaimsAsync(user, It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await this.identityService.SignUpAsync(request);

            // Assert
            this.mockUserManager.Verify(
                u => u.AddClaimsAsync(
                    user,
                    It.Is<IEnumerable<Claim>>(claims => claims.Any(c => c.Type == ClaimTypes.NameIdentifier))),
                Times.Once);
        }

        [TestMethod]
        public async Task SignUpAsync_WithDuplicateUserName_ReturnsDuplicateEmailError()
        {
            // Arrange
            var request = new SignUpRequest("testuser", "John", "Doe", "Password123!", "Password123!", CustomRole.User);
            var identityError = new IdentityError { Code = "DuplicateUserName" };

            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockUserManager
                .Setup(u => u.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            // Act
            var result = await this.identityService.SignUpAsync(request);

            // Assert
            Assert.AreEqual(IdentityServiceErrorCodes.DuplicateEmailAddress, result.ErrorCode);
        }

        [TestMethod]
        public async Task SignUpAsync_WithValidationError_ReturnsValidationError()
        {
            // Arrange
            var request = new SignUpRequest(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, CustomRole.User);
            this.mockValidator
                .Setup(v => v.ValidateAndThrow(request))
                .Throws(new ValidationException("Validation failed"));

            // Act
            var result = await this.identityService.SignUpAsync(request);

            // Assert
            Assert.AreEqual(IdentityServiceErrorCodes.ValidationError, result.ErrorCode);
        }

        [TestMethod]
        [ExpectedException(typeof(IdentityServiceException))]
        public async Task SignUpAsync_WithUnexpectedException_ThrowsIdentityServiceException()
        {
            // Arrange
            var request = new SignUpRequest("testuser", "John", "Doe", "Password123!", "Password123!", CustomRole.User);
            this.mockValidator.Setup(v => v.ValidateAndThrow(request));
            this.mockUserManager
                .Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            await this.identityService.SignUpAsync(request);
        }

        #endregion

        #region ForgotPasswordAsync Tests

        [TestMethod]
        public async Task ForgotPasswordAsync_WithValidEmail_ReturnsBase64Token()
        {
            // Arrange
            var request = new ForgotPasswordRequest("test@example.com");
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
            var resetToken = "reset-token-123";

            this.mockUserManager.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            this.mockUserManager.Setup(u => u.GetLoginsAsync(user)).ReturnsAsync(new List<UserLoginInfo>());
            this.mockUserManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(resetToken);

            // Act
            var result = await this.identityService.ForgotPasswordAsync(request);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result.Data));
        }

        [TestMethod]
        public async Task ForgotPasswordAsync_WithValidEmail_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new ForgotPasswordRequest("test@example.com");
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };

            this.mockUserManager.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            this.mockUserManager.Setup(u => u.GetLoginsAsync(user)).ReturnsAsync(new List<UserLoginInfo>());
            this.mockUserManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token");

            // Act
            var result = await this.identityService.ForgotPasswordAsync(request);

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorCode));
        }

        [TestMethod]
        public async Task ForgotPasswordAsync_WithNonExistentUser_ReturnsUserNotFoundError()
        {
            // Arrange
            var request = new ForgotPasswordRequest("nonexistent@example.com");
            this.mockUserManager.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync((User)null!);

            // Act
            var result = await this.identityService.ForgotPasswordAsync(request);

            // Assert
            Assert.AreEqual(IdentityServiceErrorCodes.UserNotFound, result.ErrorCode);
        }

        [TestMethod]
        public async Task ForgotPasswordAsync_WithSocialUser_ReturnsSocialUserError()
        {
            // Arrange
            var request = new ForgotPasswordRequest("social@example.com");
            var user = new User { Id = Guid.NewGuid(), Email = "social@example.com" };
            var socialLogins = new List<UserLoginInfo>
            {
                new UserLoginInfo("Google", "google-id", "Google"),
            };

            this.mockUserManager.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            this.mockUserManager.Setup(u => u.GetLoginsAsync(user)).ReturnsAsync(socialLogins);

            // Act
            var result = await this.identityService.ForgotPasswordAsync(request);

            // Assert
            Assert.AreEqual(IdentityServiceErrorCodes.SocialUserForgotPasswordError, result.ErrorCode);
        }

        #endregion

        #region ResetPasswordAsync Tests

        [TestMethod]
        public async Task ResetPasswordAsync_WithNonExistentUser_ReturnsUserNotFoundError()
        {
            // Arrange
            var request = new ResetPasswordRequest("nonexistent@example.com", "token", "NewPassword123!");
            this.mockUserManager.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync((User)null!);

            // Act
            var result = await this.identityService.ResetPasswordAsync(request);

            // Assert
            Assert.AreEqual(IdentityServiceErrorCodes.UserNotFound, result.ErrorCode);
        }

        [TestMethod]
        public async Task ResetPasswordAsync_WithInvalidBase64Token_ReturnsUnexpectedError()
        {
            // Arrange
            var request = new ResetPasswordRequest("test@example.com", "not-base64!", "NewPassword123!");
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };

            this.mockUserManager.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync(user);

            // Act
            var result = await this.identityService.ResetPasswordAsync(request);

            // Assert
            Assert.AreEqual(IdentityServiceErrorCodes.UnexpectedError, result.ErrorCode);
        }

        #endregion

        #region AddToRoleAsync Tests

        [TestMethod]
        [ExpectedException(typeof(IdentityServiceException))]
        public async Task AddToRoleAsync_WithException_ThrowsIdentityServiceException()
        {
            // Arrange
            var request = new AddToRoleRequest(Guid.NewGuid().ToString(), CustomRole.Admin.ToString());
            this.mockUserManager
                .Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            await this.identityService.AddToRoleAsync(request);
        }

        [TestMethod]
        public async Task AddToRoleAsync_WithException_LogsException()
        {
            // Arrange
            var request = new AddToRoleRequest(Guid.NewGuid().ToString(), CustomRole.Admin.ToString());
            this.mockUserManager
                .Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            try
            {
                await this.identityService.AddToRoleAsync(request);
            }
            catch
            {
                // Expected
            }

            // Assert
            this.mockLogger.Verify(l => l.WriteException(It.IsAny<IdentityServiceException>(), null), Times.Once);
        }

        #endregion
    }
}
