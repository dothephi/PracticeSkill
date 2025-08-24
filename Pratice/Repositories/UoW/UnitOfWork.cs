using DataAccess.Repositories;
using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UoW
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly PracticeSkillContext _dbContext;
        private bool disposed = false;
        private IProductRepository _productRepository;


        public PracticeSkillContext dbContext { get { return _dbContext; } }
        public IProductRepository ProductRepository { get { return _productRepository; } }



        public UnitOfWork(PracticeSkillContext dbcontext, IProductRepository productRepository)
        {
            _dbContext = dbcontext;
            _productRepository = productRepository;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
