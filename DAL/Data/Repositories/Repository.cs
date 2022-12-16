using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class, new()
    {
        protected DbContext Context { get; }


        public Repository(DbContext context)
        {
            this.Context = context;
        }

        #region base operations
        public IEnumerable<T> Get()
        {
            return Context.Set<T>().ToList();
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public void Edit(T entity)
        {
            Context.Entry<T>(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }
        #endregion

        #region extended operations
        public IEnumerable<T> Get(Expression<Func<T, bool>> conditions,
           params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Context.Set<T>();
            if (includes != null)
            {
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
            }
            if (conditions != null)
            {
                query = query.Where(conditions);
            }
            return query.ToList();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> conditions)
        {
            return Get(conditions, null).ToList();
        }

        public IEnumerable<T> Get(params Expression<Func<T, object>>[] includes)
        {
            return Get(null, includes).ToList();
        }

        //handige functies
        public T SearchOnPrimaryKey<TPrimaryKey>(TPrimaryKey id)
        {
            return Context.Set<T>().Find(id);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }

        public void Delete<TPrimaryKey>(TPrimaryKey id)
        {
            var entity = SearchOnPrimaryKey(id);
            Context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }

        public void DetacheObject(T entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }
        #endregion
    }
}
