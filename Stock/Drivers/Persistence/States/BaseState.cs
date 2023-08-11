using NodaTime;

namespace Stock.Drivers.Persistence.States;

public record BaseState(uint RowVersion)
{
    public bool IsDeleted { get; set; }
    public Instant CreateAt { get; set; }
    public Instant UpdateAt { get; set; }
}