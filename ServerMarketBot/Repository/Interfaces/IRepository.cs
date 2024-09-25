using ServerMarketBot.Entities.Common;
using System.Linq.Expressions;

namespace ServerMarketBot.Repository.Interfaces;

public interface IRepository<TEntity>
    where TEntity : Entity
{
    Task AddAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(Guid Id);
    Task<TEntity?> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression);
    Task<List<TEntity>> GetAllAsync();
    Task<List<TEntity>> GetAllByExpressionAsync(Expression<Func<TEntity, bool>> expression);
}
