using Application.Helpers;
using Application.Interfaces;
using Application.ViewModels.Book;
using Application.ViewModels.MemberDashboard;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MemberDashboardRepository : IMemberDashboardRepository
    {
        private readonly LibraryDbContext _libraryDbContext;
        public MemberDashboardRepository(LibraryDbContext context)
        {
            _libraryDbContext = context;
        }
        public async Task<PaginatedResult<GetAllBooksForMemberVM>> GetAllBooksAsync(string? searchTitle, string? searchGenre, string? searchAuthor, int pageNumber, int pageSize)
        {
            var books = _libraryDbContext.Books
                .AsNoTracking()
                .Include(b => b.Genres)
                .Include(b => b.Publisher)
                .AsQueryable();
            if (!string.IsNullOrEmpty(searchTitle))
            {
                books = books.Where(b => b.Name.Contains(searchTitle));
            }
            if (!string.IsNullOrEmpty(searchGenre))
            {
                books = books.Where(b => b.Genres.Any(g => g.Name.Contains(searchGenre)));
            }
            if (!string.IsNullOrEmpty(searchAuthor))
            {
                books = books.Where(b => b.Author.Contains(searchAuthor));
            }
            List<GetAllBooksForMemberVM> booksVM = new List<GetAllBooksForMemberVM>();
            foreach (var book in books)
            {
                int id = book.PublisherId;
                var Pub = _libraryDbContext.Publishers.FirstOrDefault(b => b.Id == id);
                var ge = book.Genres.Select(g => g.Name).ToList();
                var model = new GetAllBooksForMemberVM
                {
                    Id=book.Id,
                    ISBN = book.ISBN,
                    Name = book.Name,
                    Author = book.Author,
                    Description = book.Description,
                    Genres = book.Genres.Select(g => g.Name).ToList(),
                    PublisherId = book.PublisherId,
                    PublisherName = Pub.Name,

                };
                booksVM.Add(model);
            }
            var totalItems = booksVM.Count;
            var items = booksVM
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return new PaginatedResult<GetAllBooksForMemberVM>
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }

    }
}
