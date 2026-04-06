using Garmetix.Models.Bases;
using Garmetix.Models.Bharat.Enums;
using System.Linq.Expressions;

namespace Garmetix.Core.DataModels
{
    public interface IDataModel<TEntity> where TEntity : class, IEntity
    {
        int Count();

        string GetError();

        IQueryable<TEntity> Where<TKey>(Expression<Func<TEntity, bool>> predict, Order? orderby, Expression<Func<TEntity, TKey>> order);

        Task<TEntity?> SaveAsync(TEntity entity, bool isNew = true);

        Task<List<TEntity>?> SaveAllAsync(List<TEntity> values, bool isNew = true);

        Task<TEntity?> GetByIdAsync(Guid id);

        Task<TEntity?> CreateAsync(TEntity entity);

        Task<TEntity?> UpdateAsync(TEntity entity);

        Task<TEntity?> GetAsync(Guid id);

        Task<bool> DeleteAsync(Guid id);

        Task<List<TEntity>?> GetAllAsync();

        Task<List<TEntity>?> GetAllAsync(int pageNumber, int pageSize);

        Task<List<TEntity>?> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);

        Task<bool> IsExists(Guid id);

        Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null);

        int GetCount(Expression<Func<TEntity, bool>>? filter = null);

        Task<List<TEntity>?> GetAllWithoutLinkAsync();

        Task<List<TEntity>?> GetAllWithoutLinkAsync(int pageNumber, int pageSize);
    }
}