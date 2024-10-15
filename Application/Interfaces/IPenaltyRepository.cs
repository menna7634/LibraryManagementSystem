using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Enums;
using Application.Helpers;
using Application.Models;
using Application.ViewModels.Penalty;

namespace Application.Interfaces
{
    public interface IPenaltyRepository
    {
        Task<IEnumerable<Penalty>> GetPenaltiesByUserAsync(string userId);
        Task<Penalty> GetPenaltyByIdAsync(int id);
        Task<bool> DeletePenaltyAsync(int id); 

        Task<IEnumerable<Penalty>> GetPenaltiesByCheckoutAsync(int checkoutId);
        Task AddPenaltyAsync(AddPenaltyVM penaltyVM);
        Task<bool> MarkAsPaidAsync(int penaltyId);
        decimal CalculatePenaltyAmount(PenaltyType type, int overdueDays = 0);

        Task UpdatePenaltyAsync(Penalty penalty);
        Task<PaginatedResult<Penalty>> GetAllPenaltiesPaginatedAsync(int pageNumber,
            int pageSize,
            bool? isPaid = null,
            string username = null,
            int? bookId = null,
            int? bookCopyId = null);
    }

}
