using System.Linq.Expressions;

namespace DataAccess.Repositories.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(string id);
        Task AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task DeleteAsync(string id);
        IQueryable<T> GetQuery();
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetWithIncludesAsync(Expression<Func<T, bool>> predicate, string includeProperties);
        Task<T?> GettWithIncludesAsync(Expression<Func<T, bool>> predicate, string includeProperties);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
