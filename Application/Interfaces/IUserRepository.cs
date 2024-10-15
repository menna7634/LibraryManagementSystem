
using Application.Helpers;
using Application.Models;
using Application.ViewModels;
using Application.ViewModels.Member;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<SignInResponse> SignInAsync(LoginViewModel model);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);

        string GetCurrentUserId();

        Task SignOutAsync();
        Task<PaginatedResult<UserVM>> SearchUsersAsync(string searchUser, string searchEmail, DateTime? searchJoinedAt, string orderBy, int pageNumber, int pageSize);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
    }
}
