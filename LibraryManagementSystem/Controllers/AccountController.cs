using Application.Enums;
using Application.Interfaces;
using Application.Models;
using Application.ViewModels;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly LibraryDbContext _libraryDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;



        public AccountController(IUserRepository userRepository , UserManager<ApplicationUser> userManager, IEmailService emailService, IPasswordValidator<ApplicationUser> passwordValidator, LibraryDbContext libraryDbContext)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _emailService = emailService;
            _passwordValidator = passwordValidator;
            _libraryDbContext = libraryDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userRepository.RegisterAsync(model);

            if (result.Succeeded)
            {
                // Store the user's email in TempData to pass to the OTP view
                TempData["Email"] = model.Email;

                ViewBag.SuccessMessage = "Registration successful! Please check your email for the OTP.";
                return RedirectToAction("VerifyOtp"); // Redirect to the VerifyOtp action
            }

            ViewBag.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            return View(model);
        }
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            var model = new VerifyOtpViewModel
            {
                Email = TempData["Email"]?.ToString()!, // Retrieve the email from TempData
                Otp = string.Empty 
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return NotFound("User not found");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var storedOtp = HttpContext.Session.GetString("Otp");

            if (storedOtp != null && storedOtp == model.Otp) 
            {
                user.EmailConfirmed = true; 
                await _userManager.UpdateAsync(user);

                // Clear the OTP from the session after successful verification
                HttpContext.Session.Remove("Otp");

                return View("RegistrationSuccess"); 
            }

            ModelState.AddModelError("", "Invalid or expired OTP");
            return View(model);
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var signInResponse = await _userRepository.SignInAsync(model);

            if (signInResponse.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user!);

                // Generate the token
                var token = signInResponse.Token; 

                // Set token in a cookie 
                Response.Cookies.Append("jwt", token!, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, 
                    SameSite = SameSiteMode.Strict 
                });

                // Redirect based on role
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("AdminDashboard");
                }
                else if (roles.Contains("Member"))
                {
                    return RedirectToAction("Profile", "MemberDashboard");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = signInResponse.Message ?? "Invalid email or password.";
            return View(model);
        }



        [HttpGet]
        public IActionResult RequestResetPassword()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid password reset request.");
            }

            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token,
               
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> RequestResetPassword(ResetPasswordRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if the user exists
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email does not exist.");
                return View(model); 
            }

            // Generate password reset token
            var token = await _userRepository.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token },Request.Scheme);

            await _emailService.SendEmailAsync(model.Email, "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return RedirectToAction("ResetPasswordConfirmation");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId!);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            var passwordValidationResult = await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
            if (passwordValidationResult.Succeeded)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "Your password has been reset successfully. You can now log in.";
                    return View(model);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                foreach (var error in passwordValidationResult.Errors)
                {
                    ModelState.AddModelError("NewPassword", error.Description);
                }
            }

            return View(model);
        }


        public IActionResult Dashboard()
        {
            return View(); 
        }

        public async Task<IActionResult> AdminDashboard()
        {
            //counts of checkouts for chart
            var checkoutCounts = new List<int>();
            for (int month = 1; month <= 12; month++)
            {
                int count = await _libraryDbContext.Checkouts
                    .CountAsync(c => c.CheckoutDate.Month == month && c.CheckoutDate.Year == DateTime.Now.Year);
                checkoutCounts.Add(count);
            }
            ViewBag.CheckoutCounts = checkoutCounts;

            var totalBooks = await _libraryDbContext.Books.CountAsync();
            ViewBag.TotalBooks = totalBooks;

            var id = await _libraryDbContext.Roles.Where(i => i.Name == "Member").FirstAsync();
            var totalMembers = await _libraryDbContext.UserRoles.Where(r => r.RoleId == id.Id).CountAsync();
            ViewBag.totalMembers = totalMembers;

            var totalCheckouts = await _libraryDbContext.Checkouts.CountAsync();
            ViewBag.totalCheckouts = totalCheckouts;

            var PaidPenalties = await _libraryDbContext.Penalties.Where(p=>p.IsPaid==true).CountAsync();
            ViewBag.PaidPenalties = PaidPenalties;

            var UnPaidPenalties = await _libraryDbContext.Penalties.Where(p => p.IsPaid == false).CountAsync();
            ViewBag.UnPaidPenalties = UnPaidPenalties;

            var RCheckouts = await _libraryDbContext.Checkouts.Where(i=>i.Status == CheckoutStatus.Returned).CountAsync();
            ViewBag.RCheckouts = RCheckouts;

            var URCheckouts = await _libraryDbContext.Checkouts.Where(i => i.Status == CheckoutStatus.Overdue).CountAsync();
            ViewBag.URCheckouts = URCheckouts;

            var PCheckouts = await _libraryDbContext.Checkouts.Where(i => i.Status == CheckoutStatus.Pending).CountAsync();
            ViewBag.PCheckouts = PCheckouts;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userRepository.SignOutAsync(); 

            return RedirectToAction("Login", "Account");
        }


    }



}


