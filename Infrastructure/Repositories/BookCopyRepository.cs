using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Book;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BookCopyRepository : GenericRepository<BookCopy>, IBookCopyRepository
    {
        private readonly LibraryDbContext _libraryDbContext;
        public BookCopyRepository(LibraryDbContext context) : base(context)
        {
            _libraryDbContext = context;
        }

        public async Task AddBookCopyAsync(AddBookCopyVM bookCopy)
        {
            var newBookCopy = new BookCopy
            {
                BookId = bookCopy.BookId,
                Location = bookCopy.Location,
                Available = bookCopy.Available
            };
            await _libraryDbContext.BookCopies.AddAsync(newBookCopy);
            await _libraryDbContext.SaveChangesAsync();
        }

        public async Task<PaginatedResult<ViewBookCopyVM>> GetAllBookCopiesAsync(int bookId, string searchStatus, int pageNumber, int pageSize)
        {
            var bookCopies = _libraryDbContext.BookCopies
                .Where(bc => bc.BookId == bookId)
                .OrderByDescending(bc => bc.Available)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchStatus))
            {
                bool isAvailable = searchStatus == "true";
                bookCopies = bookCopies.Where(bc => bc.Available == isAvailable);
            }
            List < ViewBookCopyVM> VMs= new List<ViewBookCopyVM>();
            foreach(var bc in bookCopies)
            {
                var v = new ViewBookCopyVM
                {
                    Id = bc.Id,
                    Location = bc.Location,
                    Available = bc.Available,
                    BookId = bc.BookId,
                    BookName = bc.Book.Name
                };
                VMs.Add(v);
            }

            var totalItems = await bookCopies.CountAsync();
            var items = VMs
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            return new PaginatedResult<ViewBookCopyVM>
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
            };
        }

        public async Task<ViewBookCopyVM> GetBookCopyByIdAsync(int Id)
        {
            var bookCopy = await _libraryDbContext.BookCopies
                .Include(bc => bc.Book)
                .Select(bc => new ViewBookCopyVM
                {
                    Id = bc.Id,
                    Location = bc.Location,
                    Available = bc.Available,
                    BookId = bc.BookId,
                    BookName = bc.Book.Name
                })
                .FirstOrDefaultAsync(bc => bc.Id == Id);
            return bookCopy;
        }
        public async Task UpdateBookCopyAsync(int id, AddBookCopyVM bookCopyVM)
        {
            var bookCopy = await _libraryDbContext.BookCopies.FindAsync(id);
            bookCopy.Location = bookCopyVM.Location;
            bookCopy.Available = bookCopyVM.Available;
            await _libraryDbContext.SaveChangesAsync();
        }

        public async Task DeleteBookCopyAsync(int Id)
        {
            var bookCopy = await _libraryDbContext.BookCopies.FindAsync(Id);
            _libraryDbContext.BookCopies.Remove(bookCopy);
            await _libraryDbContext.SaveChangesAsync();
        }

    }
}
