using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.ViewModels.Book;
using Infrastructure.Data;
using Application.Models;
using Microsoft.EntityFrameworkCore;
using Application.ViewModels.Publisher;
using Infrastructure.Repositories;
using System.Drawing.Printing;
namespace LibraryManagementSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly LibraryDbContext _libraryDbContext;
        public BookController(IBookRepository bookRepository , LibraryDbContext libraryDbContext) 
        {
            _bookRepository = bookRepository;
            _libraryDbContext = libraryDbContext;
        }
        public async Task<IActionResult> Index(string? searchTitle,string? searchGenre, string? searchAuthor, string? searchStatus, int pageNumber = 1, int pageSize = 10)
        {
            var books = await _bookRepository.GetAllBooksAsync(searchTitle, searchGenre, searchAuthor, searchStatus, pageNumber, pageSize);
            if (!books.Items.Any())
            {
                ViewBag.Message = "No Books found for the specified Search criteria you provide";
            }
            ViewData["searchTitle"] = searchTitle;
            ViewData["searchGenre"] = searchGenre;
            ViewData["searchAuthor"] = searchAuthor;
            ViewData["searchStatus"] = searchStatus;
            ViewData["pageNumber"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            return View(books);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddBookVM
            {
                AvailableGenres = await _libraryDbContext.Genres
                    .Select(g => new Genre { Id = g.Id, Name = g.Name })
                    .ToListAsync(),
                AvailablePublishers = await _libraryDbContext.Publishers
                    .Select(p => new Application.ViewModels.Publisher.ViewPublisherVM { Id = p.Id, Name = p.Name })
                    .ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBookVM model)
        {
            if (ModelState.IsValid)
            {
                await _bookRepository.AddBookAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }

            var model = new AddBookVM
            {
                ISBN = book.ISBN,
                Name = book.Name,
                Author = book.Author,
                Description = book.Description,
                PublisherId = book.PublisherId,
                GenreIds = book.Genres.Select(g => g.Id).ToList(),
                AvailableGenres = await _libraryDbContext.Genres
                    .Select(g => new Genre { Id = g.Id, Name = g.Name })
                    .ToListAsync(),
                AvailablePublishers = await _libraryDbContext.Publishers
                    .Select(p => new ViewPublisherVM { Id = p.Id, Name = p.Name })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddBookVM model)
        {
            if (ModelState.IsValid)
            {
                var book = await _bookRepository.GetBookById(id);
                if (book == null)
                {
                    return NotFound();
                }
                await _bookRepository.UpdateBookAsync(id,model);
                return RedirectToAction("Index");
            }
            // Re-populate the dropdown lists in case of validation failure
            model.AvailableGenres = await _libraryDbContext.Genres
                .Select(g => new Genre { Id = g.Id, Name = g.Name })
                .ToListAsync();
            model.AvailablePublishers = await _libraryDbContext.Publishers
                .Select(p => new ViewPublisherVM { Id = p.Id, Name = p.Name })
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            await _bookRepository.DeleteBookAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetNumberOfBooks()
        {
            var bookCount = await _bookRepository.GetBookCountAsync();
            return Json(bookCount);
        }
       


    }

}
