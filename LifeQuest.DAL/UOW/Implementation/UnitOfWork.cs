using LifeQuest.DAL.Data;
using LifeQuest.DAL.Repositories.Interfaces;
using LifeQuest.DAL.UOW.Interface;
using LifeQuest.DAL.Repositories.Implementation;

namespace LifeQuest.DAL.UOW.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type,object> _repositories =new Dictionary<Type,object>();

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            if(!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<TEntity>(_context);
            }
            return (IGenericRepository<TEntity>)_repositories[type];
        }
    }
}
