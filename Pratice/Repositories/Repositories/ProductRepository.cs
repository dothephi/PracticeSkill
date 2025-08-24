using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Products>, IProductRepository
    {
        private readonly PracticeSkillContext _appDbContext;

        public ProductRepository(PracticeSkillContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // Example: Get product by int ProductId (already handled by GenericRepository)
        // public async Task<Products> GetProductByIdAsync(int productId)
        // {
        //     return await _appDbContext.Products.FindAsync(productId);
        // }
    }
}
