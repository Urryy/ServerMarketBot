using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ServerMarketBot.Entities.Common;
using ServerMarketBot.Repository.Interfaces;
using System.Linq.Expressions;

namespace ServerMarketBot.Repository.Impl;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly DatabaseContext context;
    private readonly DbSet<TEntity> entities;

    public Repository(DatabaseContext context, DbSet<TEntity> entities)
    {
        this.context = context;
        this.entities = entities;
    }

    public async Task AddAsync(TEntity entity)
    {
        await entities.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        entities.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await entities.ToListAsync();
    }

    public async Task<List<TEntity>> GetAllByExpressionAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await entities.Where(expression).ToListAsync();
    }

    public async Task<TEntity?> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await entities.FirstOrDefaultAsync(expression);
    }

    public async Task<TEntity?> GetByIdAsync(Guid Id)
    {
        return await entities.FirstOrDefaultAsync(entity => entity.Id == Id);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        entities.Update(entity);
        await context.SaveChangesAsync();
    }
}
