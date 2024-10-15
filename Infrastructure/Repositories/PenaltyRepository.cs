using Application.Enums;
using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Application.ViewModels.Penalty;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class PenaltyRepository : GenericRepository<Penalty>, IPenaltyRepository
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public PenaltyRepository(LibraryDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task AddPenaltyAsync(AddPenaltyVM penaltyVM)
        {

            var user = await _userManager.FindByNameAsync(penaltyVM.Username); 

            if (user == null)
            {
                throw new Exception("User not found."); 
            }
            var penalty = new Penalty
            {
                Type = penaltyVM.Type,
                Amount = CalculatePenaltyAmount(penaltyVM.Type),
                IssuedDate = DateTime.UtcNow,
                IsPaid = penaltyVM.IsPaid,   
                ApplicationUserId = user.Id,
                CheckoutId = penaltyVM.CheckoutId,

            };

            await AddAsync(penalty); 
        }

        public async Task<bool> MarkAsPaidAsync(int penaltyId)
        {
            var penalty = await GetByIdAsync(penaltyId); 
            if (penalty == null) return false;

            penalty.IsPaid = true;
            penalty.PaidAt = DateTime.UtcNow;

            await UpdateAsync(penalty); 

            return true;
        }

        public decimal CalculatePenaltyAmount(PenaltyType type, int overdueDays = 0)
        {
            decimal amount = type switch
            {
                PenaltyType.LateReturn => overdueDays * 5.00m, 
                PenaltyType.DamagedBook => 150.00m, 
                PenaltyType.NonReturn => 200.00m, 
                PenaltyType.UnpaidPreviousPenalties => 10.00m, 
                PenaltyType.ViolationOfLibraryPolicies => 20.00m, 
                _ => 0m
            };

            return(amount);
        }

        public async Task UpdatePenaltyAsync(Penalty penalty)
        {
           await UpdateAsync(penalty);
        }

        public async Task<PaginatedResult<Penalty>> GetAllPenaltiesPaginatedAsync(
            int pageNumber,
            int pageSize,
            bool? isPaid = null,
            string username = null!,
            int? bookId = null,
            int? bookCopyId = null)
        {
            var query = _context.Penalties.AsQueryable();


            if (isPaid.HasValue)
            {
                query = query.Where(p => p.IsPaid == isPaid.Value);
            }

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(p => p.ApplicationUser.UserName == username);
            }

            if (bookId.HasValue)
            {
                query = query.Where(p => p.Checkout.BookCopy.BookId == bookId.Value);
            }

            if (bookCopyId.HasValue)
            {
                query = query.Where(p => p.Checkout.BookCopyId == bookCopyId.Value);
            }

            var totalItems = await query.CountAsync();



            var penalties = await query
                .Include(p => p.ApplicationUser)
                .OrderByDescending(p => p.IssuedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Penalty>
            {
                Items = penalties,
                TotalItems = totalItems,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            };
        }

        public async Task<Penalty> GetPenaltyByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeletePenaltyAsync(int id)
        {
            var penalty = await GetByIdAsync(id);
            if (penalty == null) return false;

            await DeleteAsync(penalty.Id);

            return true;
        }
        public async Task<int> CountPenaltiesAsync(bool isPaid)
        {
            return await _context.Penalties.CountAsync(p => p.IsPaid == isPaid);
        }


    }
}
