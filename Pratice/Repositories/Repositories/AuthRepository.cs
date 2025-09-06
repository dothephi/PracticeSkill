using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.Models;

namespace DataAccess.Repositories
{
    public class AuthRepository : GenericRepository<SystemUserAccount>, IAuthRepository
    {
        private readonly PracticeSkillContext _appDbContext;

        public AuthRepository(PracticeSkillContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<SystemUserAccount> GetByUserName(string Username)
        {
            return await _appDbContext.SystemUserAccounts
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == Username);
        }

        public async Task<SystemUserAccount> GetByEmail(string email)
        {
            return await _appDbContext.SystemUserAccounts
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
