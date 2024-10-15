using Application.Helpers;
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
        public Task<PaginatedResult<GetAllBooksForMemberVM>> GetAllBooksAsync(string? searchTitle, string? searchGenre, string? searchAuthor, int pageNumber, int pageSize);

    }
}
