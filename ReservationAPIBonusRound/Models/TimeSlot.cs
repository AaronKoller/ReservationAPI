namespace ReservationAPIBonusRound.Models;

public class TimeSlot
{
    public int ProviderId;
    public bool IsReservationConfirmed { get; set; }
    public Guid TimeSlotId { get; set; } = Guid.NewGuid();
    public DateTimeOffset StarTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public DateTimeOffset ReservedTime { get; set; }
    public int ReservedByClientId { get; set; }
}