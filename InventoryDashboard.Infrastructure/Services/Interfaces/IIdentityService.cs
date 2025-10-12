namespace InventoryDashboard.Infrastructure.Services.Interfaces
{
    using System.Threading.Tasks;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Identity;

    public interface IIdentityService
    {
        Task<Response> SignUpAsync(SignUpRequest request);

        Task<Response<string>> SignInAsync(SignInRequest request);

        Task<Response<string>> SignInExternalAsync(SignInExternalRequest request);

        Task<Response> AddToRoleAsync(AddToRoleRequest request);

        Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request);

        Task<Response> ResetPasswordAsync(ResetPasswordRequest request);
    }
}