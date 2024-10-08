using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Book;
using Application.ViewModels.Publisher;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
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
        public async Task<Book?> GetBookById(int Id)
        {
            var book= await _libraryDbContext.Books
            .Include(b => b.Genres) // Include genres
            .FirstOrDefaultAsync(b => b.Id == Id);
            //var book = await GetByIdAsync(Id);
            return book;
        }

        public async Task<IEnumerable<ViewBookVM>> GetAllBooksAsync()
        {
            var books = await _libraryDbContext.Books
                .Include(b => b.Genres)
                .Include(b => b.Publisher) 
                .Include(b=>b.BookCopies)
                .ToListAsync();
            //var books= await GetAllAsync();
            List<ViewBookVM> booksVM=new List<ViewBookVM>();
            foreach(var book in books)
            {
                int id=book.PublisherId;
                var Pub = _libraryDbContext.Publishers.FirstOrDefault(b => b.Id == id);
                var bc = book.BookCopies
                        .Select(b => new ViewBookCopy
                        {
                            Available = b.Available,
                            Location = b.Location
                        }).ToList();
                var ge = book.Genres.Select(g => g.Name).ToList();
                var model = new ViewBookVM
                {
                    Id= book.Id,
                    ISBN = book.ISBN,
                    Name = book.Name,
                    Author = book.Author,
                    Description = book.Description,
                    Number = book.Number,
                    Date = book.Date,
                    Genres = book.Genres.Select(g => g.Name).ToList(),
                    PublisherId = book.PublisherId,
                    PublisherName = Pub.Name,
                    BookCopies = bc

                };
                booksVM.Add(model);
            }
            return booksVM;
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

        public async Task UpdateBookAsync(int id, AddBookVM bookVM)
        {
            var book=await GetBookById(id);

            book.ISBN = bookVM.ISBN;
            book.Name = bookVM.Name;
            book.Author = bookVM.Author;
            book.Description = bookVM.Description;
            book.PublisherId = bookVM.PublisherId;
            book.Number = bookVM.Number;
   
            // Update genres
            if (bookVM.GenreIds != null)
            {
                List<int> currentGenreIds = book.Genres.Select(i => i.Id).ToList();
                //List<int> bookvmgenerids = bookVM.GenreIds.ToList();
                var areGenresDifferent = !bookVM.GenreIds.OrderBy(id => id).SequenceEqual(currentGenreIds.OrderBy(id => id));
                if (areGenresDifferent)
                {
                    // old genres to remove
                    var genresToRemove = book.Genres
                        .Where(g => !bookVM.GenreIds.Contains(g.Id))
                        .ToList();

                    foreach (var genre in genresToRemove)
                    {
                        book.Genres.Remove(genre);
                    }

                    // new genres to add
                    var genresToAdd = await _libraryDbContext.Genres
                        .Where(g => bookVM.GenreIds.Contains(g.Id) && !currentGenreIds.Contains(g.Id))
                        .ToListAsync();

                    foreach (var genre in genresToAdd)
                    {
                        book.Genres.Add(genre);
                    }

                }
                
            }
            
            await UpdateAsync(book);

        }

        public async Task DeleteBookAsync(int Id)
        {
            await DeleteAsync(Id);
        }

    }
     
}
