namespace InventoryDashboard.Api.Controllers
{
    using InventoryDashboard.Api.Common.Extensions;
    using InventoryDashboard.Api.Extensions.Models.Identity;
    using InventoryDashboard.Api.Messages.Identity;
    using InventoryDashboard.Infrastructure.Messages.Identity;
    using InventoryDashboard.Infrastructure.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/login")]
    [ApiController]
    [AllowAnonymous]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService identityService;

        public IdentityController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInWebResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SignInWebResponse))]
        public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
        {
            var result = await this.identityService.SignInAsync(request);
            return this.CreateResponse(result.AsSignInWebResponse());
        }

        [HttpPost("sign-up")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SignUpWebResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SignUpWebResponse))]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpRequest request)
        {
            var result = await this.identityService.SignUpAsync(request);
            return this.CreateResponse(result.AsSignUpWebResponse());
        }

        [HttpPut("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ForgotPasswordWebResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ForgotPasswordWebResponse))]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
        {
            var result = await this.identityService.ForgotPasswordAsync(request);
            return this.CreateResponse(result.AsForgotPasswordWebResponse());
        }

        [HttpPut("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResetPasswordWebResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResetPasswordWebResponse))]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
        {
            var result = await this.identityService.ResetPasswordAsync(request);
            return this.CreateResponse(result.AsResetPasswordWebResponse());
        }
    }
}
