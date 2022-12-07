using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PawnShop.Business.Interfaces;
using PawnShop.Data.Models;

namespace PawnShop.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly PawnShopDbContext _context;

        public CategoryService()
        {
            _context = new PawnShopDbContext();
        }
        public async Task AddAsync(Categories model)
        {
            await _context.Categories.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<Categories> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                .FirstAsync(c => c.CategoryId == id);
        }

        public async Task<IEnumerable<Categories>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task DeleteAsync(Categories model)
        {
            await Task.Run(() => _context.Categories.Remove(model));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categories model)
        {
            await Task.Run(() => _context.Categories.Update(model));
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<object>> GetAllBySelection(Expression<Func<Categories, object>> selection)
        {
            return await  _context.Categories
                        .AsNoTracking()
                        .Select(selection)
                        .ToListAsync();
        }

        public async Task<IEnumerable<Categories>> GetAllWithDetails()
        {
            return await _context.Categories
                .Include(c => c.Pawnings)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Categories> GetOneByFIlter(Expression<Func<Categories, bool>> filter)
        {
            return await _context.Categories.FirstAsync(filter);
        }

        public int GetCount()
        {
            return _context.Categories.Count();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
