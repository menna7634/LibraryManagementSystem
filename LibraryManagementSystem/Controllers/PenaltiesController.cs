using Application.Enums;
using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Penalty;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [HttpGet]
        public async Task<IActionResult> EditPenalty(int id)
        {
            var penalty = await _penaltyRepository.GetPenaltyByIdAsync(id);
            if (penalty == null)
            {
                return NotFound();
            }

            var viewModel = new EditPenaltyVM
            {
                Id = penalty.Id,
                Type = penalty.Type,
                Amount = penalty.Amount,
                IssuedDate = penalty.IssuedDate,
                IsPaid = penalty.IsPaid,
                PaidAt = penalty.PaidAt,
                PenaltyTypeList = new SelectList(Enum.GetValues(typeof(PenaltyType)))

            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditPenalty(EditPenaltyVM updatedPenaltyVM)
        {

            if (!ModelState.IsValid)
            {

                var modelErrors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ModelState.AddModelError(string.Empty, $"Please correct the following errors: {modelErrors}");
                return View("EditPenalty", updatedPenaltyVM);
            }
            if (ModelState.IsValid)
            {
                var penaltyToUpdate = await _penaltyRepository.GetPenaltyByIdAsync(updatedPenaltyVM.Id);
                if (penaltyToUpdate == null)
                {
                    return NotFound();
                }

                penaltyToUpdate.Type = updatedPenaltyVM.Type;
                penaltyToUpdate.Amount = updatedPenaltyVM.Amount;
                penaltyToUpdate.IssuedDate = updatedPenaltyVM.IssuedDate;
                penaltyToUpdate.IsPaid = updatedPenaltyVM.IsPaid;
                penaltyToUpdate.PaidAt = updatedPenaltyVM.PaidAt;

                await _penaltyRepository.UpdatePenaltyAsync(penaltyToUpdate);

                TempData["Message"] = "Penalty updated successfully!";
                return RedirectToAction("GetPenalites");
            }

            return View(updatedPenaltyVM);
        }


        [HttpPost]
        public async Task<IActionResult> DeletePenalty(int id)
        {
            var penalty = await _penaltyRepository.GetPenaltyByIdAsync(id);
            if (penalty == null)
            {
                return NotFound();
            }

            await _penaltyRepository.DeletePenaltyAsync(id); 

            TempData["Message"] = "Penalty deleted successfully!";
            return RedirectToAction("GetPenalites");
        }

        [HttpGet]
        public async Task<IActionResult> MarkAsPaid(int penaltyId)
        {
            var result = await _penaltyRepository.MarkAsPaidAsync(penaltyId);

            if (!result)
            {
                TempData["ErrorMessage"] = "Penalty not found or could not be marked as paid.";
                return RedirectToAction("GetPenalites"); 
            }

            TempData["Message"] = "Penalty marked as paid successfully!";
            return RedirectToAction("GetPenalites");
        }

    }
}

