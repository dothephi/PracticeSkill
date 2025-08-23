using DataAccess.Repositories.IRepositories;
using System.Threading.Tasks;

namespace DataAccess.UoW
{
    public interface IUnitOfWork
    {
        IGenericRepository<Model.Models.Products> ProductRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
