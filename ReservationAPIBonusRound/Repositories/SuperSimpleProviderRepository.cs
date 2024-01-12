using Microsoft.Extensions.Options;
using ReservationAPIBonusRound.Models;
using ReservationAPIBonusRound.Models.AppSettings;

namespace ReservationAPIBonusRound.Repositories;

public interface IProviderRepository
{
    public void SetTimeSlotPerDay(int providerId, DateTimeOffset startTime, IEnumerable<TimeSlot> timeSlots);
    public IEnumerable<TimeSlot> GetAvailableTimeSlotsByDay(int providerId, DateTimeOffset startTime);
    public TimeSlot GetTimeSlotByGuid(Guid timeSlotId);
    public void UpdateTimeSlot(TimeSlot timeSlot);
}

public class SuperSimpleProviderRepository : IProviderRepository
{
    private readonly List<TimeSlot> _timeSlots = new();
    private readonly AppSettingsOptions _settings;

    public SuperSimpleProviderRepository(IOptions<AppSettingsOptions> settings)
    {
        _settings = settings.Value;
    }

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
            (x.ReservedTime == null || DateTimeOffset.Now - TimeSpan.FromSeconds(_settings.ReservationTimeoutInSeconds) > x.ReservedTime )
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