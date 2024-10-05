using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Book;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository 
    {
        private readonly LibraryDbContext _libraryDbContext;
        public BookRepository(LibraryDbContext context ):base(context)
        {
            _libraryDbContext = context;
        }

        public async Task AddBookAsync(AddBookVM bookVM)
        {
            var book=new Book{
                Name= bookVM.Name,
                Author=bookVM.Author,
                Date=DateTime.UtcNow,
                Description=bookVM.Description,
                ISBN=bookVM.ISBN,
                Number=bookVM.Number,
                PublisherId=bookVM.PublisherId,
                //price
            };
            // Add genres
            foreach (var genreId in bookVM.GenreIds)
            {
                var genre = await _libraryDbContext.Genres.FindAsync(genreId);
                if (genre != null)
                {
                    book.Genres.Add(genre);
                }
            }
            await AddAsync(book);
        }
    }
     
}
