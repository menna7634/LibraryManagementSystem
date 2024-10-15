
using Application.Helpers;
using Application.ViewModels.Return;

namespace Application.Interfaces
{
    public interface IReturnRepository
    {
        Task<PaginatedResult<ReturnDetailsVM>> GetReturnsAsync(string searchUser, DateTime? searchDueDate, DateTime? searchReturnDate , bool? isOverdue, string searchBook, int pageNumber, int pageSize);
    }
}
