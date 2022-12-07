using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronPdf;
using IronPdf.Rendering.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PawnShop.Business.Interfaces;
using PawnShop.Data.Models;

namespace PawnShop.Business.Services
{
    public class SQLCategoryService : ICategoryService
    {
        private readonly PawnShopDbContext _context;
        private readonly StringBuilder _html;
        public SQLCategoryService(StringBuilder html)
        {
            _context = new PawnShopDbContext();
            _html = html;
        }
        public async Task AddAsync(Categories model)
        {
            var query =
                $"<br>INSERT INTO [dbo].[Categories]([CategoryId],[Name],[Note]) VALUES(NEWID(), {model.Name}, {model.Note})";
            await _context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO [dbo].[Categories]([CategoryId],[Name],[Note]) VALUES(NEWID(), {model.Name}, {model.Note})");
            _html.Append(query);
        }


        public async Task<Categories> GetByIdAsync(Guid id)
        {
            var parameterId = new SqlParameter("@Id", id);
            var query = $"SELECT * FROM [dbo].[Categories] WHERE CategoryId = @Id";
            _html.Append(query);
            return await Task.Run(() => _context.Categories.FromSqlRaw(query, parameterId).First());
        }

        public async Task<IEnumerable<Categories>> GetAllAsync()
        {
            var query = "SELECT * FROM [dbo].[Categories]";
            _html.Append(query);
            return await Task.Run(() => _context.Categories.FromSqlRaw(query));
        }

        public async Task DeleteAsync(Categories model)
        {
            var query = $"<br>DELETE FROM [dbo].[Categories] WHERE CategoryId={model.CategoryId}";
            _html.Append(query);
            await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM [dbo].[Categories] WHERE CategoryId={model.CategoryId}");
        }

        public async Task UpdateAsync(Categories model)
        {
            var query = $"<br>UPDATE [dbo].[Categories] SET Name={model.Name}, Note={model.Note} WHERE CategoryId={model.CategoryId}";
            _html.Append(query);
            await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE [dbo].[Categories] SET Name={model.Name}, Note={model.Note} WHERE CategoryId={model.CategoryId}");
        }

        public async Task<IEnumerable<dynamic>> GetAllBySelection(Expression<Func<Categories, dynamic>> selection)
        {
            var details = await GetAllAsync();
            _html.Append("SELECT <selection> FROM Categories");
            return details.AsQueryable().Select(selection);
        }

        public async Task<IEnumerable<Categories>> GetAllWithDetails()
        {
            var query = "SELECT [dbo].Categories.[CategoryId], [Name], [Note], [Description], [PawningId], [SubmissionDate], [ReturnDate], [Sum], [Commision] " +
                        "FROM [dbo].[Categories] INNER JOIN [dbo].[Pawnings] ON [dbo].[Categories].CategoryId = [dbo].[Pawnings].CategoryId";
            _html.Append(query);
            return await _context.Categories.FromSqlRaw(query)
                .ToListAsync();
        }

        public async Task<Categories> GetOneByFIlter(Expression<Func<Categories, bool>> filter)
        {
            var details = await GetAllAsync();
            _html.Append($"SELECT * FROM Categories WHERE {filter}");
            return details.AsQueryable().First(filter);
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
