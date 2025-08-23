using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repositories;
using DataAccess.Repositories.IRepositories;
using Model.Data;
using Model.Models;

namespace DataAccess.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PracticeSkillContext _context;
        private IGenericRepository<Products> _productRepository;

        public UnitOfWork(PracticeSkillContext context)
        {
            _context = context;
        }

        public IGenericRepository<Products> ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new GenericRepository<Products>(_context);
                }
                return _productRepository;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
