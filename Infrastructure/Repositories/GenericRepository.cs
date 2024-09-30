using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly LibraryDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(LibraryDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity ?? throw new InvalidOperationException("Entity not found.");
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }

        public async Task AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding entity.", ex);
            }
        }

        public async Task UpdateAsync(T entity)
        {
           

            try
            {
                _dbSet.Update(entity);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error updating entity.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
           

            var entity = await GetByIdAsync(id);
            try
            {
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await SaveAsync();
                } else
                {
                    throw new InvalidOperationException("Entity Not Found");

                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error deleting entity.", ex);
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error saving changes.", ex);
            }
        }

       
    }

}
