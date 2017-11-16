using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace TeamGlobal.Infrastructure.BaseRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected TeamGlobalContext DbContext;

        public Repository(TeamGlobalContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public virtual void Add(T entity)
        {
            this.DbContext.Set<T>().Add(entity);
        }

        public virtual void Update(T entity)
        {
            this.DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            this.DbContext.Set<T>().Remove(entity);
        }

        public void Delete(Func<T, Boolean> where)
        {
            IEnumerable<T> objects = this.DbContext.Set<T>().Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                this.DbContext.Set<T>().Remove(obj);
        }

        public virtual T GetById(long id)
        {
            return this.DbContext.Set<T>().Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return this.DbContext.Set<T>().ToList();
        }

        public virtual IEnumerable<T> GetMany(Func<T, bool> where)
        {
            return this.DbContext.Set<T>().Where(where).ToList();
        }

        public T Get(Func<T, Boolean> where)
        {
            return this.DbContext.Set<T>().Where(where).FirstOrDefault<T>();
        }
    }
}