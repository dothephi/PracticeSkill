using Model.Models;

namespace DataAccess.Repositories.IRepositories
{
    public interface IAuthRepository : IGenericRepository<SystemUserAccount>
    {
        Task<SystemUserAccount> GetByUserName(string userName);
        //Task<SystemUserAccount> GetByEmail(string email);
    }
}
