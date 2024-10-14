using System.Diagnostics;
using Application.Models;
using Infrastructure.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {

		private readonly LibraryDbContext _context;
		private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger , LibraryDbContext context)
        {
            _logger = logger;
			_context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


		[HttpPost]
		public async Task<IActionResult> Index(ContactForm contactForm)
		{
			if (ModelState.IsValid)
			{
				contactForm.Submittedat = DateTime.Now;
				await _context.ContactForms.AddAsync(contactForm);
				await _context.SaveChangesAsync();
				ViewBag.Message = "Your message has been sent successfully.";
				ModelState.Clear();
				return View();
			}

			return View(contactForm);
		}


		public IActionResult Success()
		{
			return View();
		}
	}
}
