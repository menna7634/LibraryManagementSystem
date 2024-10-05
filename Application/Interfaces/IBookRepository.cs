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

        //public void UpdateBookAsync(Book book);

        //public void DeleteBookAsync(int Id);

        //public Task<Book> GetById(int Id);

        //public Task<List<Book>> GetAllBooksAsync();


    }
}
