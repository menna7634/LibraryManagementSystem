using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.ViewModels.Book;
using Infrastructure.Data;
using Application.Models;
using Microsoft.EntityFrameworkCore;
using Application.ViewModels.Publisher;
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
        public IActionResult Index()
        {
            return View();
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
       
    }
}
