
using Application.Interfaces;
using Application.Models;
using Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Application.Helpers;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

        public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor , IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
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
                string otp =Generateotp.GenerateOtp();

                // Store OTP in session
                _httpContextAccessor.HttpContext.Session.SetString("Otp", otp);

                // Send OTP via email
                await _emailService.SendEmailAsync(user.Email, "Your OTP" , $"Your OTP is {otp}"); 

                return result;
            }

            return result;
        }



        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<SignInResult> SignInAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return await _signInManager.PasswordSignInAsync(user.UserName, password, rememberMe, lockoutOnFailure: false);
            }

            return SignInResult.Failed;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }

}
