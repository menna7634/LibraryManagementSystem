using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Book;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class BookCopyController : Controller
    {
        private readonly LibraryDbContext _libraryDbContext;
        private readonly IBookCopyRepository _bookCopyRepository;
        public BookCopyController(LibraryDbContext libraryDbContext, IBookCopyRepository bookCopyRepository)
        {
            _libraryDbContext = libraryDbContext;
            _bookCopyRepository = bookCopyRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var book = _libraryDbContext.Books.Find(id);
            var model = new AddBookCopyVM
            {
                BookId = id 
            };

            return View("AddBookCopy",model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddBookCopyVM addBookCopyVM)
        {
            if (ModelState.IsValid)
            {
                await _bookCopyRepository.AddBookCopyAsync(addBookCopyVM);
                TempData["Message"] = "Book Added successfully!";
                return RedirectToAction("List", "BookCopy", new { id = addBookCopyVM.BookId });
            }
            return View("AddBookCopy", addBookCopyVM);
        }


        [HttpGet]
        [Route("BookCopy/List/{BookId}")]
        public async Task<IActionResult> List(int BookId,string? searchStatus, int pageNumber = 1, int pageSize = 10)
        {
            var book = _libraryDbContext.Books.Find(BookId);
            var bookCopies = await _bookCopyRepository.GetAllBookCopiesAsync(BookId, searchStatus, pageNumber, pageSize);
            if (!bookCopies.Items.Any())
            {
                ViewBag.Message = "No Book Copies found for the specified Search criteria you provide";
            }

            ViewData["searchStatus"] = searchStatus;
            ViewData["pageNumber"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            ViewData["BookId"] = BookId;
            return View("ListBookCopies", bookCopies);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bookCopy = await _libraryDbContext.BookCopies.FindAsync(id);
            var bookCopyVM = new AddBookCopyVM
            {
                Id = bookCopy.Id,
                BookId = bookCopy.BookId,
                Location = bookCopy.Location,
                Available = bookCopy.Available
            };
            return View("EditBookCopy", bookCopyVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddBookCopyVM bookCopyVM)
        {
            await _bookCopyRepository.UpdateBookCopyAsync(id, bookCopyVM);
            TempData["Message"] = "Book Copy Updated successfully!";
            var t = await _libraryDbContext.BookCopies
                .Where(bc => bc.Id == id)
                .Select(bc => bc.BookId)
                .FirstOrDefaultAsync();
            return RedirectToAction("List", "BookCopy", new { id = t });
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _libraryDbContext.BookCopies
                .Where(bc => bc.Id == id)
                .Select(bc => bc.BookId)
                .FirstOrDefaultAsync();
            await _bookCopyRepository.DeleteBookCopyAsync(id);
            TempData["Message"] = "Book Copy Deleted successfully!";
            return RedirectToAction("List", "BookCopy", new { id = t });
        }


    }
}
