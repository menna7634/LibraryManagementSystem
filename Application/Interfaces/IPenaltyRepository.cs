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
        public Task<int> CountPenaltiesAsync(bool isPaid);
        Task<Penalty> GetPenaltyByIdAsync(int id);
        Task<bool> DeletePenaltyAsync(int id); 
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
