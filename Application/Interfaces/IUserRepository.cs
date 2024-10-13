
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
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<SignInResponse> SignInAsync(LoginViewModel model);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);

        Task SignOutAsync();
        Task<PaginatedResult<UserVM>> GetUsersPaginatedAsync(int pageNumber, int pageSize);
        Task<PaginatedResult<UserVM>> SearchUsersAsync(string searchUser, string searchEmail, DateTime? searchJoinedAt, string orderBy, int pageNumber, int pageSize);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<int> GetTotalUsersAsync();
    }
}
