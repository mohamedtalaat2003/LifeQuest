using System.Linq.Expressions;
using X.PagedList;

namespace LifeQuest.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByNameAsync(string Name);


        Task AddAsync(T entity);

        Task Update(T entity);

        Task Delete(int id);

        Task<T?> GetByIdWithIncludeAsync(Expression<Func<T, bool>> predicate,params string[] includes);

        Task<IEnumerable<T>> GetAllWithIncludesAsync(Expression<Func<T, bool>> predicate,params string[] includes);

        Task<IPagedList<T>> GetPagedAsync(int pageNumber,int pageSize,Expression<Func<T, bool>> predicate,params string[] includes);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}