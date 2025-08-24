using DataAccess.Repositories.IRepositories;
using Model.Data;
using System.Threading.Tasks;

namespace DataAccess.UoW
{
    public interface IUnitOfWork
    {
        IProductRepository ProductRepository { get; }
        public Task<int> SaveChangeAsync();
        PracticeSkillContext dbContext { get; }
    }
}
