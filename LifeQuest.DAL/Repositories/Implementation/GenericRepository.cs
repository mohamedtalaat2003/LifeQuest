using LifeQuest.DAL.Data;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using X.PagedList;
using X.PagedList.Extensions;

namespace LifeQuest.DAL.Repositories.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly ApplicationDbContext _context;
        public readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task Delete(int id)
        {
            T? entity = await GetByIdAsync(id);

            if (entity == null)
                return;

            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = true;
                baseEntity.UpdateAt = DateTime.Now;

                _dbSet.Update(entity); // Soft Delete
            }
            else
            {
                _dbSet.Remove(entity); // Hard Delete
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public Task<IPagedList<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate, params string[] Includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            if (Includes != null)
            { 
                foreach (var include in Includes)
                {
                    query = query.Include(include);
                }
            }
            IPagedList<T> paged = query.Where(predicate).ToPagedList(pageNumber, pageSize);
            return Task.FromResult(paged);
        }

        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(Expression<Func<T, bool>> predicate,params string[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.Where(predicate).AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<T?> GetByNameAsync(string Name)
        {
            // Try to find a property called "Name" on the entity type
            var nameProperty = typeof(T).GetProperty("Name");
            if (nameProperty == null) return null;

            return await _dbSet.FirstOrDefaultAsync(
                e => EF.Property<string>(e, "Name") == Name);
        }

        public async Task<T?> GetByIdWithIncludeAsync(Expression<Func<T, bool>> predicate,params string[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public Task Update(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.UpdateAt = DateTime.Now;
            }

            _dbSet.Update(entity);

            return Task.CompletedTask;
        }
    }
}