using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class ReturnController : Controller
    {
       private readonly IReturnRepository _returnRepository;
        private readonly LibraryDbContext _libraryDbContext;


        public ReturnController(IReturnRepository returnRepository, LibraryDbContext libraryDbContext)
        {
            _returnRepository = returnRepository;
            _libraryDbContext = libraryDbContext;   
        }

        public async Task<IActionResult> GetReturns(
        string searchUser,
        DateTime? searchDueDate,
        DateTime? searchReturnDate,
        string searchBook,
        int pageNumber = 1,
        int pageSize = 10)
        {
            var result = await _returnRepository.GetReturnsAsync(
                searchUser,
                searchDueDate,
                searchReturnDate,
                searchBook,
                pageNumber,
                pageSize);

            if (!result.Items.Any())
            {
                ViewBag.Message = "No returns found for the specified search criteria.";
            }

            var totalReturned = await _libraryDbContext.Returns.CountAsync();

            int overdueCount = result.Items.Count(r => r.IsOverdue);

            ViewData["searchUser"] = searchUser;
            ViewData["searchDueDate"] = searchDueDate;
            ViewData["searchReturnDate"] = searchReturnDate;
            ViewData["searchBook"] = searchBook;
            ViewData["pageNumber"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            ViewData["TotalReturned"] = totalReturned;
            ViewData["OverdueCount"] = overdueCount; 

            return View(result);
        }

    }
}
