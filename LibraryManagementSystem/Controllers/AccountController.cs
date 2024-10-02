using Application.Interfaces;
using Application.Models;
using Application.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public AccountController(IUserRepository userRepository , UserManager<ApplicationUser> userManager)
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
                ViewBag.SuccessMessage = "Registration successful! You can now log in.";
                return View(); 
            }

            ViewBag.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            return View(model); 
        }

    }
}
