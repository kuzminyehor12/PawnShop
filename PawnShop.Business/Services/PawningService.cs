using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PawnShop.Business.Interfaces;
using PawnShop.Data.Models;

namespace PawnShop.Business.Services
{
    public class PawningService : IPawningService
    {
        private readonly PawnShopDbContext _context;
        private readonly StringBuilder _html;
        public PawningService(StringBuilder html)
        {
            _context = new PawnShopDbContext();
            _html = html;
        }
        public async Task AddAsync(Pawnings model)
        {
            await _context.Pawnings.AddAsync(model);
            _html.Append($"Adding Pawning: {model.PawningId}, {model.Description}, {model.SubmissionDate}, {model.ReturnDate}," +
                         $" {model.Sum}, {model.Commision}, {model.CategoryId}, {model.ClientId} <br>");
            await _context.SaveChangesAsync();
        }

        public async Task<Pawnings> GetByIdAsync(Guid id)
        {
            _html.Append($"SELECT * FROM Pawnings WHERE Pawnings.PawningId == {id}");
            return await _context.Pawnings
                .FirstAsync(p => p.PawningId == id);
        }

        public async Task<IEnumerable<Pawnings>> GetAllAsync()
        {
            _html.Append("SELECT * FROM Pawnings");
            return await _context.Pawnings
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task DeleteAsync(Pawnings model)
        {
            _html.Append($"Deleting Pawning:  {model.PawningId}, {model.Description}, {model.SubmissionDate}, {model.ReturnDate}," +
                         $" {model.Sum}, {model.Commision}, {model.CategoryId}, {model.ClientId}<br>");
            await Task.Run(() => _context.Pawnings.Remove(model));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pawnings model)
        {
            _html.Append($"Updating Pawning:  {model.PawningId}, {model.Description}, {model.SubmissionDate}, {model.ReturnDate}," +
                         $" {model.Sum}, {model.Commision}, {model.CategoryId}, {model.ClientId}<br>");
            await Task.Run(() => _context.Pawnings.Update(model));
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<dynamic>> GetAllBySelection(Expression<Func<Pawnings, dynamic>> selection)
        {
            _html.Append("SELECT <selection> FROM Pawnings " +
                         "INNER JOIN ClientData ON Pawnings.ClientId = ClientData.ClientId " +
                         "INNER JOIN Categories ON Pawnings.CategoryId = Categories.CategoryId <br>");
            return await _context.Pawnings
                .Include(p => p.Client)
                .Include(p => p.Category)
                .AsNoTracking()
                .Select(selection)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pawnings>> GetAllByFilter(Expression<Func<Pawnings, bool>> filter)
        {
            _html.Append($"SELECT * FROM Pawnings WHERE {filter} <br>");
            return await _context.Pawnings
                .Include(p => p.Client)
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(filter)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pawnings>> GetAllWithDetails()
        {
            _html.Append("SELECT * FROM Pawnings " +
                         "INNER JOIN ClientData ON Pawnings.ClientId = ClientData.ClientId " +
                         "INNER JOIN Categories ON Pawnings.CategoryId = Categories.CategoryId <br>");
            return await _context.Pawnings
                .Include(p => p.Client)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Pawnings> GetByIdWithDetails(Guid id)
        {
            _html.Append("SELECT * FROM Pawnings " +
                         "INNER JOIN ClientData ON Pawnings.ClientId = ClientData.ClientId " +
                         "INNER JOIN Categories ON Pawnings.CategoryId = Categories.CategoryId" +
                         $"WHERE Pawnings.PawningId = {id} <br>");
            return await _context.Pawnings
                .Include(p => p.Client)
                .Include(p => p.Category)
                .FirstAsync(p => p.PawningId == id);
        }

        public async Task<Pawnings> GetOneByFIlter(Expression<Func<Pawnings, bool>> filter)
        {
            _html.Append("SELECT * FROM Pawnings " +
                         "INNER JOIN ClientData ON Pawnings.ClientId = ClientData.ClientId " +
                         "INNER JOIN Categories ON Pawnings.CategoryId = Categories.CategoryId" +
                         $"WHERE {filter} TOP 1 <br>");
            return await _context.Pawnings
                .Include(p => p.Client)
                .Include(p => p.Category)
                .FirstAsync(filter);
        }

        public int GetCount()
        {
            return _context.Pawnings.Count();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
