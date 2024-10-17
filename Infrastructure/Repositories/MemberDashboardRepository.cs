using Application.Helpers;
using Application.Interfaces;
using Application.Models;
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

        public async Task<PaginatedResult<UserPenaltiesVM>> GetPenaltiesByUserIdAsync(
          string userId, int pageNumber, int pageSize, bool? isPaid = null)
        {
            var query = _libraryDbContext.Penalties.AsQueryable();

            query = query.Where(p => p.ApplicationUserId == userId);

            if (isPaid.HasValue)
            {
                query = query.Where(p => p.IsPaid == isPaid.Value);
            }

            var totalItems = await query.CountAsync();

            var penalties = await query
                .Include(p => p.Checkout)
                .ThenInclude(c => c.BookCopy)
                .ThenInclude(bc => bc.Book) 
                .OrderBy(p => p.IssuedDate) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new UserPenaltiesVM
                {
                    Type = p.Type,
                    Amount = p.Amount,
                    IssuedDate = p.IssuedDate,
                    IsPaid = p.IsPaid,
                    CheckoutId = p.CheckoutId,
                    BookTitle = p.Checkout.BookCopy.Book.Name, 
                    BookCopyID = p.Checkout.BookCopy.Id
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return new PaginatedResult<UserPenaltiesVM>
            {
                Items = penalties,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }
        public async Task<PaginatedResult<MemberHistoryVM>> GetMemberHistoryAsync(
    string userId,
    int pageNumber,
    int pageSize,
    string? bookTitle = null,
    DateTime? checkoutDate = null,
    DateTime? dueDate = null,
    string? returnStatus = null)
        {
            var query = _libraryDbContext.Checkouts
                .Where(c => c.ApplicationUserId == userId)
                .Include(c => c.BookCopy)
                .ThenInclude(bc => bc.Book)
                .Include(c => c.Return)
                .AsQueryable();

            if (!string.IsNullOrEmpty(bookTitle))
            {
                query = query.Where(c => c.BookCopy.Book.Name.Contains(bookTitle));
            }

            if (checkoutDate.HasValue)
            {
                query = query.Where(c => c.CheckoutDate.Date == checkoutDate.Value.Date);
            }

            if (dueDate.HasValue)
            {
                query = query.Where(c => c.DueDate.Date == dueDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(returnStatus))
            {
                if (returnStatus.Equals("returned", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(c => c.Return != null);
                }
                else if (returnStatus.Equals("notReturned", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(c => c.Return == null);
                }
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.CheckoutDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new MemberHistoryVM
                {
                    BookTitle = c.BookCopy.Book.Name,
                    BookCopyId = c.BookCopyId,
                    CheckoutDate = c.CheckoutDate,
                    DueDate = c.DueDate,
                    ReturnDate = c.Return.ReturnDate
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);


            return new PaginatedResult<MemberHistoryVM>
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }



    }
}
