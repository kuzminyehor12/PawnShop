using PawnShop.Oracle.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PawnShop.Business.Interfaces
{
    public interface IService<TModel> where TModel : BaseEntity
    {
        Task AddAsync(TModel model);
        Task<TModel> GetByIdAsync(decimal id);
        Task<IEnumerable<TModel>> GetAllAsync();
        Task DeleteAsync(TModel model);
        Task UpdateAsync(TModel model);
        int GetCount();
    }
}
