using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Models;

namespace Application.Interfaces
{
    public interface IContactFormRepository
    {
        Task<PaginatedResult<ContactForm>> GetContactFormsAsync(int page, int pageSize, string searchEmail, bool orderByNewest, DateTime? searchDate);
        Task<bool> DeleteContactFormAsync(int id);

    }

}
