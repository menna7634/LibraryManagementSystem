using Application.Enums;
using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Checkout;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagementSystem.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutRepository _checkoutRepository;
        private readonly LibraryDbContext _libraryDbContext;


        public CheckoutController(ICheckoutRepository checkoutRepository , LibraryDbContext libraryDbContext)
        {
            _checkoutRepository = checkoutRepository;
            _libraryDbContext = libraryDbContext;

        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new AddCheckoutVM
            {
                Books = (await _checkoutRepository.GetBooksAsync()).Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name 
                }),
                BookCopies = Enumerable.Empty<SelectListItem>(),
                Users = (await _checkoutRepository.GetUsersAsync()).Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.UserName
                })
            };

            return View("AddCheckout", viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddCheckoutVM checkoutVM)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(checkoutVM);

                var modelErrors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ModelState.AddModelError(string.Empty, $"Please correct the following errors: {modelErrors}");
                return View("AddCheckout", checkoutVM);
            }

            try
            {
                await _checkoutRepository.AddCheckoutAsync(checkoutVM);
                await _checkoutRepository.MarkBookCopyAsUnavailableAsync(checkoutVM.BookCopyID);

                TempData["SuccessMessage"] = "Checkout created successfully!";
                await PopulateDropdownsAsync(checkoutVM); 

                return View("AddCheckout", checkoutVM); 
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownsAsync(checkoutVM);
                return View("AddCheckout", checkoutVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again.");
                ModelState.AddModelError(string.Empty, ex.Message); 
                await PopulateDropdownsAsync(checkoutVM);
                return View("AddCheckout", checkoutVM);
            }
        }

        private async Task PopulateDropdownsAsync(AddCheckoutVM checkoutVM)
        {
            checkoutVM.Books = (await _checkoutRepository.GetBooksAsync()).Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            });

            checkoutVM.BookCopies = (await _checkoutRepository.GetAvailableBookCopiesByBookIdAsync(checkoutVM.BookID))
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = $"Copy ID: {b.Id}"
                });

            checkoutVM.Users = (await _checkoutRepository.GetUsersAsync()).Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName
            });
        }

        [HttpGet("api/bookcopies/{bookId}")]
        public async Task<IActionResult> GetAvailableBookCopiesByBookId(int bookId)
        {
            var bookCopies = await _checkoutRepository.GetAvailableBookCopiesByBookIdAsync(bookId);
            if (bookCopies == null)
            {
                return NotFound();
            }

            return Ok(bookCopies.Select(bc => new {
                id = bc.Id,
                bookName = bc.Book.Name 
            }));
        }

        public async Task<IActionResult> GetCheckouts(
         string searchUser,
         DateTime? searchDate,
         string searchBook,
         CheckoutStatus? SearchStatus,
         int pageNumber = 1,
         int pageSize = 10)
        {
            var result = await _checkoutRepository.GetCheckoutsAsync(
                searchUser,
                searchDate,
                searchBook,
                SearchStatus,
                pageNumber,
                pageSize);

            if (!result.Items.Any())
            {
                ViewBag.Message = "No checkouts found for the specified Search criteria you provide";
            }

            // Calculate the numbers
            var totalPending = await _libraryDbContext.Checkouts
                .CountAsync(c => c.Status == CheckoutStatus.Pending);
            var totalOverdue = await _libraryDbContext.Checkouts
                .CountAsync(c => c.Status == CheckoutStatus.Overdue);
            var totalReturned = await _libraryDbContext.Checkouts
                .CountAsync(c => c.Status == CheckoutStatus.Returned);

            ViewData["searchUser"] = searchUser;
            ViewData["searchDate"] = searchDate;
            ViewData["searchBook"] = searchBook;
            ViewData["pageNumber"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            ViewData["SearchStatus"] = SearchStatus;
            ViewData["TotalPending"] = totalPending;
            ViewData["TotalOverdue"] = totalOverdue;
            ViewData["TotalReturned"] = totalReturned;

            return View(result);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var checkout = await _checkoutRepository.GetCheckoutByIdAsync(id); 
            if (checkout == null)
            {
                return NotFound();
            }
            return View(checkout);
        }

      
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CheckoutDetailVM checkoutVM)
        {
            if (!ModelState.IsValid)
            {
                return View(checkoutVM);
            }

            try
            {
                bool isUpdated = await _checkoutRepository.UpdateCheckoutAsync(checkoutVM);
                if (isUpdated)
                {
                    ViewBag.SuccessMessage = "Checkout updated successfully.";
                }
                else
                {
                    ModelState.AddModelError("", "Checkout not found or could not be updated.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(checkoutVM);
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var checkout = await _checkoutRepository.GetCheckoutByIdAsync(id);

            if (checkout == null)
            {
                ViewBag.ErrorMessage = "The checkout record could not be found.";
                return RedirectToAction("GetCheckouts");
            }

            var viewModel = new CheckoutDetailVM
            {
                Id = checkout.Id,
                UserName = checkout.UserName,
                BookName = checkout.BookName,
                DueDate = checkout.DueDate
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isDeleted = await _checkoutRepository.DeleteCheckoutAsync(id);

            if (isDeleted)
            {
                ViewBag.SuccessMessage = "Checkout Deleted Successfully.";
            }
            else
            {
                ViewBag.ErrorMessage = "Checkout could not be deleted.";
            }

            var checkout = await _checkoutRepository.GetCheckoutByIdAsync(id);
            var viewModel = new CheckoutDetailVM
            {
                Id = id,
                UserName = checkout?.UserName ?? string.Empty,
                BookName = checkout?.BookName ?? string.Empty,
                DueDate = checkout?.DueDate ?? DateTime.Now 
            };

            return View("Delete", viewModel);
        }


    }

}
