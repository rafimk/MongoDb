using System.Linq.Expressions;
using MongoDb.Entities;

namespace MongoDb.MongoDb;

public interface IRepository<T> where T : IEntity
{
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<T> GetAsync(Guid id);
    Task<T> FindOne(Expression<Func<T, bool>> filterExpression);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(Guid id);
}