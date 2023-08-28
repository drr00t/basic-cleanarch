using Dapper.Contrib.Extensions;

namespace ProductCatalog.Drivers.Persistence;

[Table("product_stock")]
public sealed record ProductStock([property: Key]Guid ProductId, string Name, string Description
        , double Weight, double Price, double Quantity);