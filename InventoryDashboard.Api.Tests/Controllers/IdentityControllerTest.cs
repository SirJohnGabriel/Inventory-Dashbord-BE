namespace InventoryDashboard.Api.Tests.Controllers
{
    using System.Net;
    using InventoryDashboard.Api.Controllers;
    using InventoryDashboard.Infrastructure.Enums.Identity;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Identity;
    using InventoryDashboard.Infrastructure.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class IdentityControllerTest
    {
        private IdentityController target = null!;

        private Mock<IIdentityService> identityService = null!;

        [TestInitialize]
        public void Setup()
        {
            this.identityService = new Mock<IIdentityService>();

            this.target = new IdentityController(this.identityService.Object);
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        [TestMethod]
        public async Task SignInAsync_RequestIsValid_ResponseHasExpectedStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.OK;
            var webRequest = new SignInRequest("userName", "password");
            this.identityService.Setup(i => i.SignInAsync(It.IsAny<SignInRequest>())).ReturnsAsync(new Response<string>());

            // Act
            var actual = await this.target.SignInAsync(webRequest) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task SignUpAsync_RequestIsValid_ResponseHasExpectedStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.Created;
            var webRequest = new SignUpRequest("userName", "firstName", "lastName", "password", "confirmPassword", CustomRole.Admin);
            this.identityService.Setup(i => i.SignUpAsync(It.IsAny<SignUpRequest>())).ReturnsAsync(new Response());

            // Act
            var actual = await this.target.SignUpAsync(webRequest) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task ForgotPasswordAsync_RequestIsValid_ResponseHasExpectedStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.OK;
            var webRequest = new ForgotPasswordRequest("email");
            this.identityService.Setup(i => i.ForgotPasswordAsync(It.IsAny<ForgotPasswordRequest>())).ReturnsAsync(new Response<string>());

            // Act
            var actual = await this.target.ForgotPasswordAsync(webRequest) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task ResetPasswordAsync_RequestIsValid_ResponseHasExpectedStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.OK;
            var webRequest = new ResetPasswordRequest("email", "newPassword", "token");
            this.identityService.Setup(i => i.ResetPasswordAsync(It.IsAny<ResetPasswordRequest>())).ReturnsAsync(new Response());

            // Act
            var actual = await this.target.ResetPasswordAsync(webRequest) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }
    }
}
