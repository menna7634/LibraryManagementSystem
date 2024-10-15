using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class MemberDashboardController : Controller
    {
        private readonly IMemberDashboardRepository _memberDashboardRepository;
        public MemberDashboardController(IMemberDashboardRepository memberDashboardRepository)
        {
            _memberDashboardRepository = memberDashboardRepository;
        }
        public async Task<IActionResult> Index(string? searchTitle, string? searchGenre, string? searchAuthor,int pageNumber = 1, int pageSize = 10)
        {
            var books = await _memberDashboardRepository.GetAllBooksAsync(searchTitle, searchGenre, searchAuthor, pageNumber, pageSize);
            if (!books.Items.Any())
            {
                ViewBag.Message = "No Books found for the specified Search criteria you provide";
            }
            ViewData["searchTitle"] = searchTitle;
            ViewData["searchGenre"] = searchGenre;
            ViewData["searchAuthor"] = searchAuthor;
            ViewData["pageNumber"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            return View("GetAllBooksForMember", books);
        }
    }
}
