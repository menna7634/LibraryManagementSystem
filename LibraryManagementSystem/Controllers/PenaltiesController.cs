using Application.Enums;
using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class PenaltiesController : Controller
    {
        private readonly IPenaltyRepository _penaltyRepository;
        public PenaltiesController(IPenaltyRepository penaltyRepository)
        {
            _penaltyRepository = penaltyRepository;
        }

        public async Task<IActionResult> GetPenalites(
            int pageNumber=1,
            int pageSize=10,
            bool? isPaid = null,
            string username = null!,
            int? bookId = null,
            int? bookCopyId = null)
        {
            var result = await _penaltyRepository.GetAllPenaltiesPaginatedAsync(
            pageNumber,
            pageSize,
            isPaid,
           username,
            bookId, bookCopyId);

            if (!result.Items.Any())
            {
                ViewBag.Message = "No checkouts found for the specified Search criteria you provide";
            }

         

            ViewData["searchUser"] = username;
            ViewData["searchisPaid"] = isPaid;
            ViewData["searchBook"] = bookId;
            ViewData["searchbookCopy"] = bookCopyId;
            ViewData["pageNumber"] = pageNumber;
            ViewData["pageSize"] = pageSize;
          

            return View(result);
        }
    }
}
