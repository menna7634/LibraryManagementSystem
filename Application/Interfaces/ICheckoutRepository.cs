using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Enums;
using Application.Helpers;
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
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<PaginatedResult<CheckoutDetailVM>> GetCheckoutsAsync(string searchUser, DateTime? searchDate, string searchBook, CheckoutStatus? searchStatus, int pageNumber, int pageSize);
        Task<CheckoutDetailVM> GetCheckoutByIdAsync(int id);
        Task<bool> UpdateCheckoutAsync(CheckoutDetailVM checkoutVM);
        Task<bool> DeleteCheckoutAsync(int id);


    }

}
