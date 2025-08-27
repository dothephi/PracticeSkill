using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.Models;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories
{
    public class AuthRepository : GenericRepository<SystemUserAccount>, IAuthRepository
    {
        private readonly PracticeSkillContext _appDbContext;

        public AuthRepository(PracticeSkillContext appDbContext) : base(appDbContext)
        {
            {
                _appDbContext = appDbContext;
            }
        }

        public async Task<SystemUserAccount> GetByUserName(string Username)
        {
            return await _appDbContext.SystemUserAccounts.FirstOrDefaultAsync(u => u.Username == Username);
        }
    }
}
