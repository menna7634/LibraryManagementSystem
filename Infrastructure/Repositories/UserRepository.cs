
using Application.Interfaces;
using Application.Models;
using Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Application.Helpers;
using Microsoft.EntityFrameworkCore;
using Application.ViewModels.Member;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly LibraryDbContext _dbContext;

        public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, IEmailService emailService, ITokenService tokenService, LibraryDbContext dbcontext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _tokenService = tokenService;
            _dbContext = dbcontext;
        }
        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already exists." });
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(model.FullName);
            if (existingUserByUsername != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Username already exists." });
            }

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth,
                IsBlocked = false,
                JoinedAT = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Member");

                // Generate OTP
                string otp = Generateotp.GenerateOtp();

                // Store OTP in session
                _httpContextAccessor.HttpContext.Session.SetString("Otp", otp);

                // Send OTP via email
                await _emailService.SendEmailAsync(user.Email, "Your OTP", $"Your OTP is {otp}");

                return result;
            }

            return result;
        }



        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<SignInResponse> SignInAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new SignInResponse
                {
                    Succeeded = false,
                    Message = "Invalid email or password."
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.GenerateToken(user, roles.ToList());

                return new SignInResponse
                {
                    Succeeded = true,
                    Token = token
                };
            }
            return new SignInResponse
            {
                Succeeded = false,
                Message = "Invalid email or password."
            };
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<PaginatedResult<UserVM>> GetUsersPaginatedAsync(int pageNumber, int pageSize)
        {
            // Get the "User" RoleId
            var userRoleId = await _dbContext.Roles
                .Where(r => r.Name == "User")  // Fetching the role ID for "User"
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // Query to get users who have the "User" role only
            var query = from user in _dbContext.Users
                        join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                        where userRole.RoleId == userRoleId  // Only users with the "User" role
                        select new UserVM
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            JoinedAt = user.JoinedAT,
                            Gender = user.Gender,
                            Address = user.Address,
                            DateOfBirth = user.DateOfBirth
                        };

            var totalCount = await query.CountAsync(); // Get total count of "User" role users

            // Pagination logic
            var users = await query
                .OrderBy(u => u.UserName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Return paginated result
            return new PaginatedResult<UserVM>
            {
                Items = users,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalItems = totalCount
            };
        }


        // Search users by username, email, joined date

        public async Task<PaginatedResult<UserVM>> SearchUsersAsync(string searchUser, string searchEmail, DateTime? searchJoinedAt, string orderBy, int pageNumber, int pageSize)
        {
            // Get the "User" RoleId
            var userRoleId = await _dbContext.Roles
                .Where(r => r.Name == "User")  // Fetching the role ID for "User"
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // Query to get users with "User" role and apply search filters
            var query = from user in _dbContext.Users
                        join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                        where userRole.RoleId == userRoleId  // Only users with the "User" role
                        select user;

            // Apply search filters
            if (!string.IsNullOrEmpty(searchUser))
            {
                query = query.Where(u => u.UserName.Contains(searchUser));
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                query = query.Where(u => u.Email.Contains(searchEmail));
            }

            if (searchJoinedAt.HasValue)
            {
                query = query.Where(u => u.JoinedAT.Date == searchJoinedAt.Value.Date);
            }

            var totalItems = await query.CountAsync(); // Get total count after filters

            var users = await query
                .OrderBy(j => j.JoinedAT)  // Modify ordering as needed
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserVM
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    JoinedAt = u.JoinedAT,
                    Gender = u.Gender,
                    Address = u.Address,
                    DateOfBirth = u.DateOfBirth
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Return the paginated result
            return new PaginatedResult<UserVM>
            {
                Items = users,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }



        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _dbContext.Users
                   .FirstOrDefaultAsync(u => u.Id == userId);
        }


        public async Task<int> GetTotalUsersAsync()
        {
            return await _dbContext.Users.CountAsync();
        }

    }
}


