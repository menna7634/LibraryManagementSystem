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

        //public Task UpdateBookCopyAsync(int id, AddBookCopyVM bookCopy);
        //public Task<ViewBookCopy> GetBookCopyById(int Id);
        //public Task DeleteBookCopyAsync(int Id);
    }
}
