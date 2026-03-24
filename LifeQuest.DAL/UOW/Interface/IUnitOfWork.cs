using LifeQuest.DAL.Repositories.Interfaces;

namespace LifeQuest.DAL.UOW.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> CompleteAsync();
    }
}