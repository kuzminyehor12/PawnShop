using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PawnShop.Data.Models;

namespace PawnShop.Business.Interfaces
{
    public interface IPawningService : IService<Pawnings>, IAsyncDisposable
    {
        Task<IEnumerable<dynamic>> GetAllBySelection(Expression<Func<Pawnings, dynamic>> selection);
        Task<IEnumerable<Pawnings>> GetAllByFilter(Expression<Func<Pawnings, bool>> filter);
        Task<IEnumerable<Pawnings>> GetAllWithDetails();
        Task<Pawnings> GetByIdWithDetails(Guid id);
        Task<Pawnings> GetOneByFIlter(Expression<Func<Pawnings, bool>> filter);
    }
}
