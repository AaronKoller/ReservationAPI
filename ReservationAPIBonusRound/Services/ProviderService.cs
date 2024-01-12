using Microsoft.Extensions.Options;
using ReservationAPIBonusRound.Models;
using ReservationAPIBonusRound.Models.AppSettings;
using ReservationAPIBonusRound.Repositories;

namespace ReservationAPIBonusRound.Services;

public interface IProviderService
{
    public IEnumerable<TimeSlot> GetAvailableTimeSlots(int providerId, DateTimeOffset startTime);
    public bool CreateTimeSlots(int providerId, DateTimeOffset startTime, DateTimeOffset endTime);
    public bool ReserveTimeSlot(Guid timeSlotId, int clientId);
    public bool ConfirmReservation(Guid timeSlotId, int clientId);
}

//TODO: replace all false return with proper Error Handling.  Some bool return types may be converted to Voids 
public class ProviderService : IProviderService
{
    private readonly IProviderRepository _providerRepository;
    private readonly AppSettingsOptions _settings;
    public ProviderService(IOptions<AppSettingsOptions> settings,  IProviderRepository providerRepository)
    {
        _settings = settings.Value;
        _providerRepository = providerRepository;
    }

    public IEnumerable<TimeSlot> GetAvailableTimeSlots(int providerId, DateTimeOffset startTime)
    {
        return _providerRepository.GetAvailableTimeSlotsByDay(providerId, startTime);
    }

    public bool CreateTimeSlots(int providerId, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        //TODO move guard clauses FluentValidations
        if (startTime.Day != endTime.Day) return false;
        if (startTime.Minute % _settings.MinimumTimeBlockInSeconds != 0 || endTime.Minute % _settings.MinimumTimeBlockInSeconds != 0) return false;

        var truncatedStartTime = new DateTimeOffset(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 0, 0, startTime.Offset);
        var truncatedEndTime = new DateTimeOffset(endTime.Year, endTime.Month, endTime.Day, endTime.Hour, endTime.Minute, 0, 0, endTime.Offset);

        var timeSlots = GenerateTimeSlots(providerId, truncatedStartTime, truncatedEndTime);
        _providerRepository.SetTimeSlotPerDay(providerId, truncatedStartTime, timeSlots);

        return true;
    }

    private IEnumerable<TimeSlot> GenerateTimeSlots(int providerId, DateTimeOffset truncatedStartTime, DateTimeOffset truncatedEndTime)
    {
        var timeSlots = new List<TimeSlot>();
        var slotEndTime = DateTimeOffset.MinValue;

        var slotStartTime = truncatedStartTime;
        while (slotEndTime < truncatedEndTime)
        {
            slotEndTime = slotStartTime.AddSeconds(_settings.MinimumTimeBlockInSeconds);
            timeSlots.Add(new TimeSlot
            {
                ProviderId = providerId,
                StarTime = slotStartTime,
                EndTime = slotEndTime
            });
            slotStartTime = slotEndTime;
        }

        return timeSlots;
    }

    public bool ReserveTimeSlot(Guid timeSlotId, int clientId)
    {
        var timeSlot = _providerRepository.GetTimeSlotByGuid(timeSlotId);
        if (timeSlot.StarTime < DateTimeOffset.Now + TimeSpan.FromDays(_settings.MinimumAdvancedReservationInDays))
        {
            return false;
        }

        timeSlot.ReservedTime = DateTimeOffset.Now;
        timeSlot.ReservedByClientId = clientId;
        _providerRepository.UpdateTimeSlot(timeSlot);
        return true;
    }

    public bool ConfirmReservation(Guid timeSlotId, int clientId)
    {
        var timeSlot = _providerRepository.GetTimeSlotByGuid(timeSlotId);
        if (timeSlot.ReservedByClientId != clientId)
        {
            return false;
        }

        timeSlot.IsReservationConfirmed = true;
        _providerRepository.UpdateTimeSlot(timeSlot);
        return true;
    }
}