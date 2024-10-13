using Application.Helpers;
using Application.Models;
using Application.ViewModels.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBookRepository 
    {
        public Task AddBookAsync(AddBookVM book);
        public Task UpdateBookAsync(int id, AddBookVM bookVM);

        public Task<Book> GetBookById(int Id);
        public Task DeleteBookAsync(int Id);
        public Task<int> GetBookCountAsync();
        public Task<bool> IsBookAvailableAsync(int bookId);
        public Task<PaginatedResult<ViewBookVM>> GetAllBooksAsync(string? searchTitle, string? searchGenre, string? searchAuthor, string? searchStatus, int pageNumber, int pageSize);



    }
}
