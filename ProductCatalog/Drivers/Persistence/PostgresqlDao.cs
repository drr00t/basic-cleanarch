using System.Data.Common;
using Dapper.Contrib.Extensions;
using Npgsql;
using ProductCatalog.Capabilities.Persistence;
using ProductCatalog.Capabilities.Supporting;

namespace ProductCatalog.Drivers.Persistence;

public class PostgresqlDao:IDao<ProductStock>
{
    private readonly DbDataSource _dataSource;
    private const string ProductChangelog = "ProductChangelog";
    
    public PostgresqlDao(IConfig config)
    {
        var connectionString = config.FromEnvironment(ProductChangelog);

        if (!connectionString.IsSucceded)
            throw new ArgumentNullException(ProductChangelog);
            
        _dataSource = NpgsqlDataSource.Create(connectionString.Succeded);
    }

    public async Task Insert(ProductStock product)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var tran = await conn.BeginTransactionAsync();
        await tran.Connection.InsertAsync(product);
        await tran.CommitAsync();
    }

    public async Task<ProductStock> GetBy(Guid productId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        return await conn.GetAsync<ProductStock>(productId.ToString());
    }
}