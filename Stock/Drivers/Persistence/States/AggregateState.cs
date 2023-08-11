using System.Text.Json;
using NodaTime;

namespace Stock.Capabilities.Persistence.States;

public sealed record AggregateState(Guid Id
    , Guid AggregateId
    , string AggregationType
    , string EventType
    , Instant EventDatetime
    , JsonDocument EventData):IDisposable
{
    public void Dispose()
    {
        EventData.Dispose();
    }
}