using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PawnShop.Business.DTO;
using PawnShop.Business.Interfaces;
using PawnShop.Data.Models;

namespace PawnShop.Business.Services
{
    public class ClientPassportService : IClientPassportService
    {
        private readonly PawnShopDbContext _context;
        private readonly StringBuilder _html;
        public ClientPassportService(StringBuilder html)
        {
            _context = new PawnShopDbContext();
            _html = html;
        }
        public async Task AddAsync(ClientPassportDTO dto)
        {
            string[] names = dto.FullName.Split(' ');

            var passport = new PassportData
            {
                PassportIdataId = Guid.NewGuid(),
                Number = dto.PassportNumber,
                Series = dto.PassportSeries,
                DateOfIssue = dto.DateOfIssue
            };

            await _context.PassportData.AddAsync(passport);
            _html.Append($"Adding Passport: {passport.PassportIdataId}, {passport.Number}, {passport.Series}, {passport.DateOfIssue} <br>");

            var client = new ClientData
            {
                ClientId = Guid.NewGuid(),
                Surname = names[0],
                Name = names[1],
                Patronymic = names.Length == 3 ? names[2] : null,
                PassportData = passport
            };

            await _context.ClientData.AddAsync(client);
            _html.Append($"Adding Client: {client.ClientId}, {client.FullName}, {passport.PassportIdataId} <br>");

            await _context.SaveChangesAsync();
        }

        public async Task<ClientPassportDTO> GetByIdAsync(Guid id)
        {
            var client = await _context.ClientData
                .Include(c => c.PassportData)
                .FirstAsync(c => c.ClientId == id);

            _html.Append(
                $"SELECT * FROM ClientData INNER JOIN PassportData ON ClientData.PassportIdataId = PassportData.PassportIdataId WHERE ClientData.ClientId = {id} <br>");

            return Mapper.Map(client);
        }

        public async Task<IEnumerable<ClientPassportDTO>> GetAllAsync()
        {
            var clients = await _context.ClientData
                                        .AsNoTracking()
                                        .Include(c => c.PassportData)
                                        .ToListAsync();

            _html.Append("SELECT * FROM ClientData INNER JOIN PassportData ON ClientData.PassportIdataId = PassportData.PassportIdataId <br>");

            return Mapper.Map(clients);
        }

        public async Task DeleteAsync(ClientData model)
        {
            await Task.Run(() => _context.ClientData.Remove(model));
            _html.Append($"Deleting Client: {model.ClientId}, {model.FullName}, {model.PassportDataId} <br>");
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ClientPassportDTO dto)
        {
            string[] names = dto.FullName.Split(' ');

            var passport = new PassportData
            {
                PassportIdataId = dto.PassportId,
                Number = dto.PassportNumber,
                Series = dto.PassportSeries,
                DateOfIssue = dto.DateOfIssue
            };

            await Task.Run(() => _context.PassportData.Update(passport));
            _html.Append($"Updating Passport: {passport.PassportIdataId}, {passport.Number}, {passport.Series}, {passport.DateOfIssue} <br>");

            var client = new ClientData
            {
                ClientId = dto.ClientId,
                Surname = names[0],
                Name = names[1],
                Patronymic = names.Length == 3 ? names[2] : null,
                PassportData = passport
            };

            await Task.Run(() => _context.ClientData.Update(client));
            _html.Append($"Updating Client: {client.ClientId}, {client.FullName}, {client.PassportDataId} <br>");
            await _context.SaveChangesAsync();
        }

        public async Task<PassportData> GetPassport(Expression<Func<PassportData, bool>> filter)
        {
            _html.Append($"SELECT * FROM PassportData WHERE {filter} <br>");
            return await _context.PassportData
                .AsNoTracking()
                .FirstAsync(filter);
        }

        public async Task<ClientData> GetClient(Expression<Func<ClientData, bool>> filter)
        {
            _html.Append($"SELECT * FROM ClientData INNER JOIN PassportData ON ClientData.PassportIdataId = PassportData.PassportIdataId WHERE {filter}<br>");
            return await _context.ClientData
                .Include(c => c.PassportData)
                .AsNoTracking()
                .FirstAsync(filter);
        }

        public int GetCount()
        {
            return _context.ClientData.Count();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
