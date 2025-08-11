using ECommerceAPI.Application.Abstractions.Services.Authentication;

namespace ECommerceAPI.Application.Abstractions.Services
{
    public interface IAuthService : IExternalAuthentication, IInternalAuthentication
    {
        Task PasswordResetAsync(string email);
        Task<bool> VerifyResetToken(string resetToken, string userId);
    }
}
