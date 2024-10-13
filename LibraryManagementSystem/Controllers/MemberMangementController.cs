using Application.Interfaces;
using Application.ViewModels.Member;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class MemberMangementController : Controller
    {
        private readonly IUserRepository _userRepository;

        public MemberMangementController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: MemberMangement
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            
           var result = await _userRepository.GetUsersPaginatedAsync(pageNumber, pageSize);

            ViewData["searchUser"] = string.Empty; // Or set to a default value if needed
            ViewData["searchEmail"] = string.Empty; // Set as needed
            ViewData["searchJoinedAt"] = null; // Set as needed
            ViewData["TotalUsers"] = result.TotalCount; // Assuming your result has TotalCount property

            return View("GetMembers",result); // Pass the result to the view
        }

        // GET: MemberMangement/Search
        [HttpGet]
        public async Task<IActionResult> Search(string searchUser, string searchEmail, DateTime? searchJoinedAt, string orderBy = "UserName", int pageNumber = 1, int pageSize = 10)
        {
            var result = await _userRepository.SearchUsersAsync(searchUser, searchEmail, searchJoinedAt, orderBy, pageNumber, pageSize);
            return View("GetMembers", result); // Reuse Index view for displaying search results
        }

        // GET: MemberMangement/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user); // Pass the user to the details view
        }

       
    }
}

