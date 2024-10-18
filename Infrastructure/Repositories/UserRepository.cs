
using Application.Interfaces;
using Application.Models;
using Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Application.Helpers;
using Microsoft.EntityFrameworkCore;
using Application.ViewModels.Member;
using Infrastructure.Data;
using System.Security.Claims;
using System.Net.Http;

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

            var existingUserByPhoneNumber = await _userManager.Users
              .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (existingUserByPhoneNumber != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Phone number already exists." });
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
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Response.Cookies.Delete("jwt");
            }
        }



        public async Task<PaginatedResult<UserVM>> SearchUsersAsync(string searchUser, string searchEmail, DateTime? searchJoinedAt, string orderBy, int pageNumber, int pageSize)
        {
            var userRoleId = await _dbContext.Roles
                .Where(r => r.Name == "Member")  
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var query = from user in _dbContext.Users
                        join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                        where userRole.RoleId == userRoleId 
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

            var totalItems = await query.CountAsync(); 

            var users = await query
                .OrderBy(j => j.JoinedAT)  
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
                    DateOfBirth = u.DateOfBirth,
                    PhoneNumber=u.PhoneNumber
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);


            var memberCount = await _userManager.Users
      .Join(_dbContext.UserRoles,
            user => user.Id,
            userRole => userRole.UserId,
            (user, userRole) => new { user, userRole })
      .Where(x => x.userRole.RoleId == userRoleId)
      .CountAsync();

            // Return the paginated result
            return new PaginatedResult<UserVM>
            {
                Items = users,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalItems = totalItems,
                TotalCount = memberCount
            };
        }



        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _dbContext.Users
                   .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public string GetCurrentUserId(HttpContext httpContext)
        {
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                return userIdClaim?.Value ?? string.Empty;
            }
            return string.Empty; 
        }




    }
}


