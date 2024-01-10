using ReservationAPI.Models;

namespace ReservationAPI.Repositories;

public interface IProviderRepository
{
    public void SetTimeSlotPerDay(int providerId, DateTimeOffset startTime, IEnumerable<TimeSlot> timeSlots);
    public IEnumerable<TimeSlot> GetAvailableTimeSlotsByDay(int providerId, DateTimeOffset startTime);
    public TimeSlot GetTimeSlotByGuid(Guid timeSlotId);
    public void UpdateTimeSlot(TimeSlot timeSlot);
}

//simple 
public class SuperSimpleProviderRepository : IProviderRepository
{
    private readonly List<TimeSlot> _timeSlots = new();

    //TODO refactor this configuration to use IOptions<T> and pull from appSettings
    private readonly TimeSpan _reservationExpirationTime = TimeSpan.FromMinutes(30);
    public void SetTimeSlotPerDay(int providerId, DateTimeOffset startTime, IEnumerable<TimeSlot> timeSlots)
    {
        _timeSlots.RemoveAll(x => x.ProviderId == providerId && x.StarTime.Day == startTime.Day);
        _timeSlots.AddRange(timeSlots);
    }

    public IEnumerable<TimeSlot> GetAvailableTimeSlotsByDay(int providerId, DateTimeOffset startTime)
    {
        return _timeSlots.Where(x => 
            x.ProviderId == providerId && 
            x.StarTime.Date == startTime.Date &&
            !x.IsReservationConfirmed &&
            (x.ReservedTime == null || DateTimeOffset.Now - _reservationExpirationTime > x.ReservedTime )
            ).ToList();
    }

    public TimeSlot GetTimeSlotByGuid(Guid timeSlotId)
    {
        return _timeSlots.First(x => x.TimeSlotId == timeSlotId);
    }

    public void UpdateTimeSlot(TimeSlot timeSlot)
    {
        var index = _timeSlots.IndexOf(timeSlot);
        if (index != -1)
        {
            _timeSlots[index] = timeSlot;
        }
    }
}