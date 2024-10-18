using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Application.ViewModels.Member;
using Microsoft.VisualBasic;
using Application.Enums;

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
        public async Task<IActionResult> Index(string? searchTitle, string? searchGenre, string? searchAuthor, int pageNumber = 1, int pageSize = 10)
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

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = _userRepository.GetCurrentUserId(HttpContext);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var userVM = new UserVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                JoinedAt = user.JoinedAT,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
            };
            var CD = DateTime.Today;
            var d=CD.AddDays(3);
            var books = await _libraryDbContext.Checkouts.Where(i=>i.ApplicationUserId==userId).CountAsync();//T
            var penalities = _libraryDbContext.Penalties.Where(i => i.ApplicationUserId == userId).Where(i=>i.IsPaid==false).Sum(a=>a.Amount);//T
            var currentCheclouts = await _libraryDbContext.Checkouts.Where((i => i.ApplicationUserId == userId && i.Status != CheckoutStatus.Returned)).CountAsync();//T
            var over = await _libraryDbContext.Checkouts.Where((u=>u.ApplicationUserId==userId)).ToListAsync();
            var overdue = over.Where(i => i.DueDate >= CD && i.DueDate <= d && i.Status == CheckoutStatus.Pending).Count();
            ViewData["books"] = books;
            ViewData["penalties"] = penalities;
            ViewData["currentCheckouts"] = currentCheclouts;
            ViewData["overdue"] = overdue;
            ViewData["Days"] = d;
            ViewData["Days2"] = CD;
            return View(userVM);
        }

        public async Task<IActionResult> GetWishlist(int pageNumber = 1, int pageSize = 10, string? searchTitle = null, string? searchGenre = null)
        {
            var userId = _userRepository.GetCurrentUserId(HttpContext);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }
            var result = await _memberDashboardRepository.GetWishlistAsync(userId, pageNumber, pageSize, searchTitle, searchGenre);

            if (!result.Items.Any())
            {
                ViewBag.Message = "No Books found for the specified Search criteria you provide";
            }

            var genres = await _libraryDbContext.Genres.ToListAsync();
            ViewData["Genres"] = new SelectList(genres, "Name", "Name");
            ViewData["searchTitle"] = searchTitle;
            ViewData["searchGenre"] = searchGenre;

            return View(result); 
        }
    }
}
