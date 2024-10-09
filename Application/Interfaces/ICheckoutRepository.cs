using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;
using Application.ViewModels.Checkout;

namespace Application.Interfaces
{
	public interface ICheckoutRepository
	{
        Task<Checkout> AddCheckoutAsync(AddCheckoutVM checkoutVM);
        Task<BookCopy?> GetBookCopyById(int id);
        Task<IEnumerable<BookCopy>> GetAvailableBookCopiesByBookIdAsync(int bookId);
        Task<bool> MarkBookCopyAsUnavailableAsync(int bookCopyId);
        Task<IEnumerable<ApplicationUser>> GetUsersAsync();
        Task<IEnumerable<Book>> GetBooksAsync();


    }

}
