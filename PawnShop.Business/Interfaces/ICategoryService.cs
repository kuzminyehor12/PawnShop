using PawnShop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PawnShop.Business.Interfaces
{
    public interface ICategoryService : IService<Categories>, IAsyncDisposable
    {
        Task<IEnumerable<dynamic>> GetAllBySelection(Expression<Func<Categories, dynamic>> selection);
        Task<IEnumerable<Categories>> GetAllWithDetails();
        Task<Categories> GetOneByFIlter(Expression<Func<Categories, bool>> filter);
    }
}
