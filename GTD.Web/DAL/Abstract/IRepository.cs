using System;
using System.Linq;
using System.Linq.Expressions;

namespace GTD.DAL.Abstract
{
    public interface IRepository<TEntity> : IDisposable  where TEntity : class
    {
        void Create(TEntity instance);
        void Update(TEntity instance);
        void Delete(TEntity instance);
        TEntity Get(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAll();
        void SaveChanges();
    }
}
