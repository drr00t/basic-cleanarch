using NodaTime;

namespace Stock.Drivers.Persistence.States;

public sealed record ProductState(Guid Id, Guid ProductId, string Name, string Description
        , float Weight, float Price, int Quantity, uint RowVersion)
    : BaseState(RowVersion);