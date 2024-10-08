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
        public Task<IEnumerable<ViewBookVM>> GetAllBooksAsync();
        public Task DeleteBookAsync(int Id);


    }
}
