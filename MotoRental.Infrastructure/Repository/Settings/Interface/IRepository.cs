using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository.Settings.Interface
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task AddAsync(TEntity entity, bool includeKey = false);
        Task<TIdentity> AddGetIdentityAsync<TIdentity>(TEntity entity, bool includeKey = false);
        Task<TEntity> AddGetEntityAsync(TEntity entity, bool includeKey = false);
        Task DeleteAsync(Expression<Func<TEntity, bool>> expression);
        Task DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, object>> orderBy = null, bool ascending = true);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> orderBy = null, bool ascending = true);
        Task<TEntity> GetByIdAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> orderBy = null, bool ascending = true);
        Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> expression);
        Task UpdateAsync(TEntity entity);
        Task SaveChangesAsync();
        void Rollback();
    }
}
