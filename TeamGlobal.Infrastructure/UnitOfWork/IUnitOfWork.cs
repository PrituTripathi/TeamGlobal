using System;
using TeamGlobal.Infrastructure.BaseRepository;

namespace TeamGlobal.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        bool Commit();
    }
}