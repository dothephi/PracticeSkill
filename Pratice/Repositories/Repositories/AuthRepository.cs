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
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(PracticeSkillContext appDbContext, ILogger<AuthRepository> logger) : base(appDbContext)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<SystemUserAccount> GetByUserName(string Username)
        {
            var user = await _appDbContext.SystemUserAccounts
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == Username);
            if (user == null)
                _logger.LogWarning($"User not found for username: {Username}");
            else
                _logger.LogInformation($"User found: {user.Username}, Hash: {user.PasswordHash}");
            return user;
        }
    }
}
