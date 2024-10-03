using Application.Interfaces;
using Application.Models;
using Application.ViewModels;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public AccountController(IUserRepository userRepository , UserManager<ApplicationUser> userManager )
        {
            _userRepository = userRepository;
            _userManager = userManager;

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
                Email = TempData["Email"]?.ToString(), // Retrieve the email from TempData
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

                // Redirect based on role
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (roles.Contains("Member"))
                {
                    return RedirectToAction("Index", "Member");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = signInResponse.Message ?? "Invalid email or password.";
            return View(model);
        }








    }
}

