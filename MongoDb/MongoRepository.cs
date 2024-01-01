using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDb.Entities;

namespace MongoDb.MongoDb;

public class MongoRepository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> _dbCollection;
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;
    
    public MongoRepository(IMongoDatabase database, string collectionName) => 
        _dbCollection = database.GetCollection<T>(collectionName);
    
    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
    }

    public async Task<T> GetAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(entity => entity.Id, id);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }
    
    public async Task<T> FindOne(Expression<Func<T, bool>> filterExpression)
    {
        return await _dbCollection.Find(filterExpression).FirstOrDefaultAsync();
    }


    public async Task CreateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var filter = _filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
        await _dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(entity => entity.Id, id);
        await _dbCollection.DeleteOneAsync(filter);
    }
}