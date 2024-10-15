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
    }
}
