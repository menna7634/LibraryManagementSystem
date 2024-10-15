
using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Checkout;
using Application.ViewModels.Return;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReturnReository : IReturnRepository
    {
        private readonly LibraryDbContext _libraryDbContext;

        public ReturnReository(LibraryDbContext context)
        {
            _libraryDbContext = context;
        }

        public async Task<PaginatedResult<ReturnDetailsVM>> GetReturnsAsync(
      string searchUser,
      DateTime? searchDueDate,
      DateTime? searchReturnDate,
      bool? isOverdue,
      string searchBook,
      int pageNumber,
      int pageSize)
        {
            var query = _libraryDbContext.Returns
                .Include(r => r.Checkout)
                    .ThenInclude(c => c.ApplicationUser)
                .Include(r => r.Checkout.BookCopy)
                    .ThenInclude(bc => bc.Book)
                .AsQueryable();

            // Apply filters based on user input
            if (!string.IsNullOrEmpty(searchUser))
            {
                query = query.Where(r => r.Checkout.ApplicationUser.UserName.Contains(searchUser));
            }

            if (searchDueDate.HasValue)
            {
                query = query.Where(r => r.Checkout.DueDate.Date == searchDueDate.Value.Date);
            }

            if (searchReturnDate.HasValue)
            {
                query = query.Where(r => r.ReturnDate.Date == searchReturnDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(searchBook))
            {
                query = query.Where(r => r.Checkout.BookCopy.Book.Name.Contains(searchBook));
            }
            if (isOverdue.HasValue)
            {
                query = query.Where(r => (r.ReturnDate > r.Checkout.DueDate) == isOverdue.Value);
            }
            var totalItems = await query.CountAsync();

            var paginatedResults = await query
                .OrderByDescending(r => r.ReturnDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReturnDetailsVM
                {
                    CheckoutId = r.CheckoutId,
                    UserName = r.Checkout.ApplicationUser.UserName,
                    BookName = r.Checkout.BookCopy.Book.Name,
                    BookCopyId = r.Checkout.BookCopyId,
                    DueDate = r.Checkout.DueDate,
                    ReturnDate = r.ReturnDate,
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);


            return new PaginatedResult<ReturnDetailsVM>
            {
                Items = paginatedResults,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                TotalItems = totalItems,
            };
        }

    }
}
