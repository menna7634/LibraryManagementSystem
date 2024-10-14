using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ContactFormRepository : GenericRepository<ContactForm>, IContactFormRepository
    {
        private readonly LibraryDbContext _context;

        public ContactFormRepository(LibraryDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<ContactForm>> GetContactFormsAsync(int pageNumber, int pageSize, string searchEmail, bool orderByNewest , DateTime? searchDate)
        {
            var query = _context.ContactForms.AsQueryable();


            if (!string.IsNullOrEmpty(searchEmail))
            {
                query = query.Where(cf => cf.Email.Contains(searchEmail));
            }

            if (searchDate.HasValue)
            {
                query = query.Where(c => c.Submittedat.Date == searchDate.Value.Date);
            }
            query = orderByNewest
                ? query.OrderByDescending(cf => cf.Submittedat) 
                : query.OrderBy(cf => cf.Submittedat);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var totalcount=_context.ContactForms.Count();

            return new PaginatedResult<ContactForm>
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                TotalCount = totalcount
            };
        }

        public async Task<bool> DeleteContactFormAsync(int id)
        {
            var contactForm = await GetByIdAsync(id);
            if (contactForm == null) return false;

            await DeleteAsync(contactForm.Id);

            return true;
        }
    }

}
