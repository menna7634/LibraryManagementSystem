using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Book;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
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

    }
}
