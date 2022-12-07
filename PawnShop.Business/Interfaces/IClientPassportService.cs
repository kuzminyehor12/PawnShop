using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PawnShop.Business.DTO;
using PawnShop.Data.Models;

namespace PawnShop.Business.Interfaces
{
    public interface IClientPassportService : IAsyncDisposable
    {
        Task AddAsync(ClientPassportDTO dto);
        Task<ClientPassportDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<ClientPassportDTO>> GetAllAsync();
        Task DeleteAsync(ClientData model);
        Task UpdateAsync(ClientPassportDTO model);
        Task<PassportData> GetPassport(Expression<Func<PassportData, bool>> filter);
        Task<ClientData> GetClient(Expression<Func<ClientData, bool>> filter);
        int GetCount();
    }
}
