using Application.Helpers;
using Application.ViewModels.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBookCopyRepository
    {
        public Task AddBookCopyAsync(AddBookCopyVM bookCopy);
        public Task<PaginatedResult<ViewBookCopyVM>> GetAllBookCopiesAsync(int bookId, string? searchStatus, int pageNumber, int pageSize);
        public Task<ViewBookCopyVM> GetBookCopyByIdAsync(int Id);
        public Task UpdateBookCopyAsync(int id, AddBookCopyVM bookCopyVM);
        public Task DeleteBookCopyAsync(int Id);

    }
}
