using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class MemberDashboardController : Controller
    {
        private readonly IMemberDashboardRepository _memberDashboardRepository;
        private readonly IUserRepository _userRepository;
        private readonly LibraryDbContext _libraryDbContext;
        public MemberDashboardController(IMemberDashboardRepository memberDashboardRepository, IUserRepository userRepository, LibraryDbContext libraryDbContext)
        {
            _memberDashboardRepository = memberDashboardRepository;
            _userRepository = userRepository;
            _libraryDbContext = libraryDbContext;
        }
        public async Task<IActionResult> Index(string? searchTitle, string? searchGenre, string? searchAuthor,int pageNumber = 1, int pageSize = 10)
        {
            var genres = await _libraryDbContext.Genres.ToListAsync();
            ViewData["Genres"] = new SelectList(genres, "Name", "Name");
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

        [HttpGet]
        public async Task<IActionResult> GetUserPenalties(int pageNumber = 1, int pageSize = 10, bool? isPaid = null)
        {
            var userId = _userRepository.GetCurrentUserId(HttpContext);
           
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }
            var paginatedPenalties = await _memberDashboardRepository.GetPenaltiesByUserIdAsync(userId, pageNumber, pageSize, isPaid);

            ViewData["isPaid"] = isPaid; 
            return View(paginatedPenalties);

        }


        [HttpGet]
        public async Task<IActionResult> GetUserHistory(
    int pageNumber = 1,
    int pageSize = 10,
    string? bookTitle = null,
    DateTime? checkoutDate = null,
    DateTime? dueDate = null,
    string? returnStatus = null)
        {
            var userId = _userRepository.GetCurrentUserId(HttpContext);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            var history = await _memberDashboardRepository.GetMemberHistoryAsync(
                userId,
                pageNumber,
                pageSize,
                bookTitle,
                checkoutDate,
                dueDate,
                returnStatus);

            ViewData["bookTitle"] = bookTitle;
            ViewData["checkoutDate"] = checkoutDate;
            ViewData["dueDate"] = dueDate;
            ViewData["returnStatus"] = returnStatus;

            return View(history);
        }


    }
}
