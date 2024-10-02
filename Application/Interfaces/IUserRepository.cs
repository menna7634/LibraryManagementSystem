
using Application.Models;
using Application.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<SignInResult> SignInAsync(string email, string password, bool rememberMe);
        Task SignOutAsync();
    }
}
