namespace ProductCatalog.Capabilities.Persistence;

public interface IDao<TEntity>
{
    Task Insert(TEntity entity);
    Task<TEntity> GetBy(Guid productId);
}