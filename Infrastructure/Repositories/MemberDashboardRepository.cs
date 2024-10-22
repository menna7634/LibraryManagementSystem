using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Book;
using Application.ViewModels.MemberDashboard;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Infrastructure.Repositories
{
    public class MemberDashboardRepository : IMemberDashboardRepository
    {
        private readonly LibraryDbContext _libraryDbContext;
        private readonly IUserRepository _userRepository;

        public MemberDashboardRepository(LibraryDbContext context ,IUserRepository userRepository )
        {
            _libraryDbContext = context;
            _userRepository = userRepository;
        }

        public async Task<PaginatedResult<GetAllBooksForMemberVM>> GetAllBooksAsync(string userId, string? searchTitle, string? searchGenre, string? searchAuthor, int pageNumber, int pageSize)
        {
            var booksQuery = _libraryDbContext.Books
                .AsNoTracking()
                .Include(b => b.Genres)
                .Include(b => b.Publisher)
                .Include(b => b.WishlistBooks)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTitle))
            {
                booksQuery = booksQuery.Where(b => b.Name.Contains(searchTitle));
            }
            if (!string.IsNullOrEmpty(searchGenre))
            {
                booksQuery = booksQuery.Where(b => b.Genres.Any(g => g.Name.Contains(searchGenre)));
            }
            if (!string.IsNullOrEmpty(searchAuthor))
            {
                booksQuery = booksQuery.Where(b => b.Author.Contains(searchAuthor));
            }

            var wishlistForUser = await _libraryDbContext.WishlistBooks
                .Where(wb => wb.Wishlist.UserId == userId)
                .Select(wb => wb.BookId)
                .ToListAsync();

            var paginatedBooks = await booksQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var booksVM = paginatedBooks.Select(book => new GetAllBooksForMemberVM
            {
                Id = book.Id,
                ISBN = book.ISBN,
                Name = book.Name,
                Author = book.Author,
                Description = book.Description,
                Genres = book.Genres.Select(g => g.Name).ToList(),
                PublisherId = book.PublisherId,
                PublisherName = book.Publisher.Name,
                IsInWishlist = wishlistForUser.Contains(book.Id) 
            }).ToList();

            var totalItems = await booksQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return new PaginatedResult<GetAllBooksForMemberVM>
            {
                Items = booksVM,
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
        public async Task<PaginatedResult<WishlistVM>> GetWishlistAsync(string userId, int pageNumber, int pageSize , string? searchTitle, string? searchGenre)
        {
            var query = _libraryDbContext.WishlistBooks
                .Include(wb => wb.Book)
                .ThenInclude(b => b.Genres)
                .Where(wb => wb.Wishlist.UserId == userId).AsQueryable();
            if (!string.IsNullOrEmpty(searchTitle))
            {
                query = query.Where(b => b.Book.Name.Contains(searchTitle));
            }
            if (!string.IsNullOrEmpty(searchGenre))
            {
                query = query.Where(b => b.Book.Genres.Any(g => g.Name.Contains(searchGenre)));
            }
            var totalItems = await query.CountAsync();

            var items = await query
           .Skip((pageNumber - 1) * pageSize)
           .Take(pageSize)
          .Select(wb => new WishlistVM
          {
              Title = wb.Book.Name,
              Author = wb.Book.Author,
              Genres = wb.Book.Genres.Select(g => g.Name).ToList(),

          })
           .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return new PaginatedResult<WishlistVM>
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }

        public async Task<bool> ToggleWishlistAsync(string userId, int bookId)
        {
            var wishlist = await _libraryDbContext.Wishlists
                .Include(w => w.WishlistBooks)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                    WishlistBooks = new List<WishlistBook>()
                };
                _libraryDbContext.Wishlists.Add(wishlist);
            }

            var wishlistBook = wishlist.WishlistBooks.FirstOrDefault(wb => wb.BookId == bookId);

            if (wishlistBook != null)
            {
                wishlist.WishlistBooks.Remove(wishlistBook);
            }
            else
            {
                wishlist.WishlistBooks.Add(new WishlistBook
                {
                    BookId = bookId
                });
            }

            await _libraryDbContext.SaveChangesAsync();
            return wishlistBook == null; 
        }


    }
}
