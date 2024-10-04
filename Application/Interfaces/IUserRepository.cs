
using Application.Helpers;
using Application.Models;
using Application.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<SignInResponse> SignInAsync(LoginViewModel model);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);

        Task SignOutAsync();
    }
}
