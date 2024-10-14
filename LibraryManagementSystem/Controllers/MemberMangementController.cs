using Application.Interfaces;
using Application.ViewModels.Member;
using Microsoft.AspNetCore.Identity;
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


        // GET: MemberMangement/Search
        [HttpGet]
        public async Task<IActionResult> Search(string searchUser, string searchEmail, DateTime? searchJoinedAt, string orderBy = "UserName", int pageNumber = 1, int pageSize = 10)
        {
            var result = await _userRepository.SearchUsersAsync(searchUser, searchEmail, searchJoinedAt, orderBy, pageNumber, pageSize);
            if (!result.Items.Any())
            {
                ViewBag.Message = "No Users found for the specified Search criteria you provide";
            }

            ViewData["pageNumber"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            ViewData["searchUser"] = searchUser;
            ViewData["searchEmail"] = searchEmail;
            ViewData["searchJoinedAt"] = searchJoinedAt;
            ViewData["TotalUsers"] = result.TotalCount;

            return View("GetMembers", result); 
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

            return View(user); 
        }

       
    }
}

