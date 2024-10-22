using Application.Helpers;
using Application.Models;
using Application.ViewModels.MemberDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMemberDashboardRepository
    {
        public Task<PaginatedResult<GetAllBooksForMemberVM>> GetAllBooksAsync(string userId, string? searchTitle, string? searchGenre, string? searchAuthor, int pageNumber, int pageSize);
        Task<PaginatedResult<UserPenaltiesVM>> GetPenaltiesByUserIdAsync(string userId, int pageNumber, int pageSize, bool? isPaid = null);
        Task<PaginatedResult<MemberHistoryVM>> GetMemberHistoryAsync(string userId, int pageNumber, int pageSize, string? bookTitle = null, DateTime? checkoutDate = null, DateTime? dueDate = null, string? returnStatus = null);
        Task<bool> ToggleWishlistAsync(string userId, int bookId);

        Task<PaginatedResult<WishlistVM>> GetWishlistAsync(string userId, int pageNumber, int pageSize,string? searchTitle, string? searchGenre);



    }
}
