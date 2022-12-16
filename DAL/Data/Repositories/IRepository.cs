using MODELS.VehicleServerInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data.Repositories
{
    public interface IRepository<T>
        where T : class, new()
    {
        IEnumerable<T> Get();

        void Add(T entity);

        void Edit(T entity);

        void Delete(T entity);

        #region extension
        IEnumerable<T> Get(Expression<Func<T, bool>> voorwaarden);

        IEnumerable<T> Get(params Expression<Func<T, object>>[] includes);

        IEnumerable<T> Get(Expression<Func<T, bool>> voorwaarden,
            params Expression<Func<T, object>>[] includes);
        #endregion

        #region usefull functions
        T SearchOnPrimaryKey<TPrimaryKey>(TPrimaryKey id);

        void AddRange(IEnumerable<T> entities);

        void Delete<TPrimaryKey>(TPrimaryKey id);

        void DeleteRange(IEnumerable<T> entities);

        void DetacheObject(T entity);
        #endregion
    }
}
