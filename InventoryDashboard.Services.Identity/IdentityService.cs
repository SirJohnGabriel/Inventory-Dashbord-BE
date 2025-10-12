namespace InventoryDashboard.Services.Identity
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using InventoryDashboard.Infrastructure.Constants.Errors;
    using InventoryDashboard.Infrastructure.Entities.Identity;
    using InventoryDashboard.Infrastructure.Enums.Identity;
    using InventoryDashboard.Infrastructure.Exceptions;
    using InventoryDashboard.Infrastructure.Logging;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Identity;
    using InventoryDashboard.Infrastructure.Services.Interfaces;
    using InventoryDashboard.Infrastructure.Validations;
    using InventoryDashboard.Services.Identity.Extensions;
    using InventoryDashboard.Services.Identity.Extensions.Models;
    using InventoryDashboard.Services.Identity.Helpers;
    using InventoryDashboard.Services.Identity.Workflows;
    using InventoryDashboard.Services.Identity.Workflows.SignInExternal;
    using Microsoft.AspNetCore.Identity;

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<Role> roleManager;
        private readonly JwtHelper jwtHelper;
        private readonly Validator validator;
        private readonly IdentityServiceWorkflows workflows;
        private readonly ILogger logger;

        public IdentityService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            JwtHelper jwtHelper,
            Validator validator,
            IdentityServiceWorkflows workflows,
            ILogger logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.jwtHelper = jwtHelper;
            this.validator = validator;
            this.workflows = workflows;
            this.logger = logger;
        }

        public async Task<Response> AddToRoleAsync(AddToRoleRequest request)
        {
            try
            {
                var result = new Response();
                result = await this.roleManager.AddToRoleAsync(this.userManager, request);
                return result;
            }
            catch (Exception ex)
            {
                var exec = new IdentityServiceException(ex);
                this.logger.WriteException(exec);
                throw exec;
            }
        }

        public async Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var result = new Response<string>();
            var user = await this.userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                result.SetError(IdentityServiceErrorCodes.UserNotFound);
                return result;
            }
            else
            {
                var socialUser = await this.userManager.GetLoginsAsync(user);

                if (socialUser.Count != 0)
                {
                    result.SetError(IdentityServiceErrorCodes.SocialUserForgotPasswordError);
                    return result;
                }

                var token = await this.userManager.GeneratePasswordResetTokenAsync(user);
                var bytes = Encoding.UTF8.GetBytes(token);

                result.Data = Convert.ToBase64String(bytes);

                return result;
            }
        }

        public async Task<Response> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var result = new Response();
            var user = await this.userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                result.SetError(IdentityServiceErrorCodes.UserNotFound);
                return result;
            }
            else
            {
                try
                {
                    var bytes = Convert.FromBase64String(request.Token);
                    var resetResult = await this.userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(bytes), request.NewPassword);

                    if (!resetResult.Succeeded)
                    {
                        result.SetError(IdentityServiceErrorCodes.InvalidToken);
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    result.SetError(IdentityServiceErrorCodes.UnexpectedError);
                    var exec = new IdentityServiceException(ex);
                    this.logger.WriteException(exec);

                    return result;
                }
            }

            return result;
        }

        public async Task<Response<string>> SignInAsync(SignInRequest request)
        {
            var result = new Response<string>();

            try
            {
                this.validator.ValidateAndThrow(request);

                var signInResult = await this.signInManager.PasswordSignInAsync(request.UserName, request.Password, false, false);

                if (!signInResult.Succeeded)
                {
                    result.SetError(IdentityServiceErrorCodes.InvalidCredential);
                    return result;
                }

                var user = await this.userManager.FindByNameAsync(request.UserName);

                if (user.Status == Infrastructure.Enums.Identity.UserStatus.Suspended)
                {
                    result.SetError(IdentityServiceErrorCodes.UserSuspended);
                    return result;
                }

                if (user.Status == Infrastructure.Enums.Identity.UserStatus.Disabled)
                {
                    result.SetError(IdentityServiceErrorCodes.UserDisabled);
                    return result;
                }

                var claims = (await this.userManager.GetClaimsAsync(user)).ToList();

                if (!string.IsNullOrWhiteSpace(user.FirstName))
                {
                    claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
                }

                if (!string.IsNullOrWhiteSpace(user.LastName))
                {
                    claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
                }

                result.Data = this.jwtHelper.GenerateToken(claims);
                return result;
            }
            catch (ValidationException ex)
            {
                result.SetError(IdentityServiceErrorCodes.ValidationError, ex.Message);
            }
            catch (Exception ex)
            {
                var exec = new IdentityServiceException(ex);
                this.logger.WriteException(exec);
                throw exec;
            }

            return result;
        }

        public async Task<Response<string>> SignInExternalAsync(SignInExternalRequest request)
        {
            try
            {
                var result = new Response<string>();
                var payload = this.jwtHelper.ValidateToken(request.IdToken, request.Provider);

                if (Enum.TryParse(request.Provider, out Provider providerEnum))
                {
                    if (payload == null && providerEnum == Provider.MICROSOFT)
                    {
                        result.SetError(IdentityServiceErrorCodes.MicrosoftTokenValidationError);
                        return result;
                    }
                    else if (payload == null && providerEnum == Provider.GOOGLE)
                    {
                        result.SetError(IdentityServiceErrorCodes.GoogleTokenValidationError);
                        return result;
                    }
                }

                var workflowRequest = new SignInExternalWorkflowRequest(request, result);
                result = await this.workflows.CreateSignInExternalWorkflow().ExecuteAsync(workflowRequest);

                return result;
            }
            catch (Exception ex)
            {
                var exec = new IdentityServiceException(ex);
                this.logger.WriteException(exec);
                throw exec;
            }
        }

        public async Task<Response> SignUpAsync(SignUpRequest request)
        {
            var result = new Response();

            try
            {
                this.validator.ValidateAndThrow(request);
                var identityResult = await this.userManager.CreateAsync(request.AsUser(), request.Password);

                if (!identityResult.Succeeded)
                {
                    identityResult.HandleIdentityResultError(ref result);
                    return result;
                }

                var createdUser = await this.userManager.FindByNameAsync(request.UserName);
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, createdUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, createdUser.UserName),
                };

                await this.userManager.AddClaimsAsync(createdUser, claims);
                await this.AddToRoleAsync(new AddToRoleRequest(createdUser.Id.ToString(), request.DefaultRole.ToString()));

                return result;
            }
            catch (ValidationException ex)
            {
                result.SetError(IdentityServiceErrorCodes.ValidationError, ex.Message);
            }
            catch (Exception ex)
            {
                var exec = new IdentityServiceException(ex);
                this.logger.WriteException(exec);
                throw exec;
            }

            return result;
        }
    }
}