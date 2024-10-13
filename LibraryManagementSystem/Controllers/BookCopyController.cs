using Application.Interfaces;
using Application.ViewModels.Book;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

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
                return RedirectToAction("Index", "Book");
            }
            return View("AddBookCopy", addBookCopyVM);
        }

    }
}
