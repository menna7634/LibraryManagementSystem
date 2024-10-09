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
    }
}
