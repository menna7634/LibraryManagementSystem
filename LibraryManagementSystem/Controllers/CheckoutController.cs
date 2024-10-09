using Application.Interfaces;
using Application.ViewModels.Checkout;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutRepository _checkoutRepository;

        public CheckoutController(ICheckoutRepository checkoutRepository)
        {
            _checkoutRepository = checkoutRepository;
        }

        // GET: Checkout/Create
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


        // POST: Checkout/Create

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
                // Handle general exceptions
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again.");
                ModelState.AddModelError(string.Empty, ex.Message); 
                await PopulateDropdownsAsync(checkoutVM);
                return View("AddCheckout", checkoutVM);
            }
        }





        public async Task<IActionResult> GetAvailableBookCopies(int bookId)
        {
            var bookCopies = (await _checkoutRepository.GetAvailableBookCopiesByBookIdAsync(bookId))
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = $"Copy ID: {b.Id}"
                });

            return Json(bookCopies);
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

    }
}
