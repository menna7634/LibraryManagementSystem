using Application.Enums;
using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Checkout;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
	public class CheckoutRepository : GenericRepository<Checkout>, ICheckoutRepository
	{
		private readonly LibraryDbContext _libraryDbContext;

		public CheckoutRepository(LibraryDbContext context) : base(context)
		{
			_libraryDbContext = context;
		}

        public async Task<Checkout> AddCheckoutAsync(AddCheckoutVM checkoutVM)
        {
            var bookCopy = await _libraryDbContext.BookCopies
                .Include(bc => bc.Book)
                .FirstOrDefaultAsync(bc => bc.Id == checkoutVM.BookCopyID);

            var user = await _libraryDbContext.Users
                .FirstOrDefaultAsync(u => u.Id == checkoutVM.UserID.ToString());

            if (bookCopy == null || user == null)
            {
                throw new InvalidOperationException("Selected book copy or user not found.");
            }

            var checkout = new Checkout
            {
                CheckoutDate = DateTime.UtcNow,
                DueDate = checkoutVM.DueDate,
                ApplicationUserId = user.Id,
                BookCopyId = bookCopy.Id,

            };

            await AddAsync(checkout);

            // Mark the book copy as unavailable
            await MarkBookCopyAsUnavailableAsync(bookCopy.Id);

            checkoutVM.BookName = bookCopy.Book.Name;
            checkoutVM.UserName = user.UserName;

            return checkout;
        }

        public async Task<BookCopy?> GetBookCopyById(int id)
        {
            return await _libraryDbContext.BookCopies
                .Include(bc => bc.Checkouts)
                .FirstOrDefaultAsync(bc => bc.Id == id);
        }


        public async Task<IEnumerable<BookCopy>> GetAvailableBookCopiesByBookIdAsync(int bookId)
        {
            return await _libraryDbContext.BookCopies
         .Include(bc => bc.Book)
         .Where(bc => bc.Available && bc.BookId == bookId) 
         .ToListAsync();
        }

        public async Task<bool> MarkBookCopyAsUnavailableAsync(int bookCopyId)
        {
            var bookCopy = await GetBookCopyById(bookCopyId);

            if (bookCopy == null)
            {
                throw new InvalidOperationException("Book copy not found.");
            }

            bookCopy.Available = false;
            _libraryDbContext.BookCopies.Update(bookCopy);
            await _libraryDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            return await _libraryDbContext.Users.ToListAsync();
        }
        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _libraryDbContext.Books.ToListAsync();
        }

        public async Task<PaginatedResult<CheckoutDetailVM>> GetCheckoutsAsync(
          string searchUser,
          DateTime? searchDate,
          string searchBook,
         CheckoutStatus? searchStatus,
          int pageNumber,
          int pageSize)
        {
            var query = _libraryDbContext.Checkouts
                .Include(c => c.ApplicationUser)
                .Include(c => c.BookCopy.Book)
                .AsQueryable();

            foreach (var checkout in query)
            {
                if (checkout.DueDate < DateTime.Now && checkout.Status != CheckoutStatus.Overdue)
                {
                    checkout.Status =CheckoutStatus.Overdue;
                }
            }

            if (!string.IsNullOrEmpty(searchUser))
            {
                query = query.Where(c => c.ApplicationUser.UserName!.Contains(searchUser));
            }

            if (searchDate.HasValue)
            {
                query = query.Where(c => c.DueDate.Date == searchDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(searchBook))
            {
                query = query.Where(c => c.BookCopy.Book.Name.Contains(searchBook));
            }
            if (searchStatus.HasValue)
            {
                query = query.Where(c => c.Status == searchStatus.Value);
            }
            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.DueDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CheckoutDetailVM
                {
                    Id = c.Id,
                    UserName = c.ApplicationUser.UserName!,
                    BookName = c.BookCopy.Book.Name,
                    BookCopyID = c.BookCopyId,
                    DueDate = c.DueDate,
                    Status = c.Status
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return new PaginatedResult<CheckoutDetailVM>
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }


        public async Task<CheckoutDetailVM> GetCheckoutByIdAsync(int id)
        {
            var checkout = await _libraryDbContext.Checkouts
                .Include(c => c.ApplicationUser)
                .Include(c => c.BookCopy.Book)
                .Where(c => c.Id == id)
                .Select(c => new CheckoutDetailVM
                {
                    Id = c.Id,
                    UserName = c.ApplicationUser.UserName!,
                    BookName = c.BookCopy.Book.Name,
                    BookCopyID = c.BookCopyId,
                    DueDate = c.DueDate,
                   
                })
                .FirstOrDefaultAsync();

            return checkout;
        }

        public async Task<bool> UpdateCheckoutAsync(CheckoutDetailVM checkoutVM)
        {
            var checkout = await GetByIdAsync(checkoutVM.Id);
            if (checkout == null) return false;

            checkout.DueDate = checkoutVM.DueDate;

            if (checkoutVM.Status.HasValue)
            {
                checkout.Status = checkoutVM.Status.Value;

                // If the status is Returned, create a return record 
                if (checkout.Status == Application.Enums.CheckoutStatus.Returned)
                {
                    var returnRecord = new Return
                    {
                        CheckoutId = checkout.Id,
                        ReturnDate = DateTime.Now,
                    };
                    await _libraryDbContext.Returns.AddAsync(returnRecord);

                    var bookCopy = await GetBookCopyById(checkout.BookCopyId);
                    if (bookCopy != null)
                    {
                        bookCopy.Available = true; // Mark the book copy as available
                        _libraryDbContext.BookCopies.Update(bookCopy);
                    }
                }
            }

            await UpdateAsync(checkout);
            await _libraryDbContext.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteCheckoutAsync(int id)
        {
            var checkout = await GetByIdAsync(id);
            if (checkout == null) return false;

           await DeleteAsync(checkout.Id);
           
            return true;
        }
    }
}
