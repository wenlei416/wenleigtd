using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using GTD.DAL.Abstract;

namespace GTD.DAL
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private GTDContext _context = new GTDContext();

        public void Create(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                _context.Set<TEntity>().Add(instance);
                SaveChanges();
            }
        }

        public void Update(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                _context.Entry(instance).State = EntityState.Modified;
                SaveChanges();
            }
        }

        public void Delete(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                _context.Entry(instance).State = EntityState.Deleted;
                SaveChanges();
            }
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().FirstOrDefault(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_context == null) return;
            _context.Dispose();
            _context = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}