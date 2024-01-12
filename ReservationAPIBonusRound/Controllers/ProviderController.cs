using Microsoft.AspNetCore.Mvc;
using ReservationAPIBonusRound.Models;
using ReservationAPIBonusRound.Services;

namespace ReservationAPIBonusRound.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;

        public ProviderController( IProviderService providerService)
        {
            _providerService = providerService;
        }

        //TODO: Create controllers to create a provider and client.  At this time any integer is accepted for a clientId and ProviderID
        //TODO: Add authorization decorators to set permissions per endpoint.  For example only a Provider should be allowed to create new TimeSlots
        //TODO: Refactor these to return IActionResult for more robust HTTP status codes and error handling
        /// <summary>
        /// Create new time slots in set 15 minute increments.  Currently times must be set at the 15, 30 ,45, 0 minute marks. Start time and end times must also be on the same day
        /// Returns 'true' if slots are successfully created
        /// </summary>
        /// <param name="providerId">Any integer works. TODO: implement provider Create</param>
        /// <param name="startTime">UTC time in the following format '2024-01-12T08:00:00.000Z' </param>
        /// <param name="endTime">UTC time in the following format '2024-01-12T16:00:00.000Z' </param>
        /// <returns></returns>
        [HttpPost(nameof(CreateTimeSlots))]
        public bool CreateTimeSlots(int providerId, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            return _providerService.CreateTimeSlots(providerId, startTime, endTime);
        }

        /// <summary>
        /// Returns all time slots for a given provider on a given day.
        /// Does not show times that are currently reserved unless the 30 minute time window has elapsed.
        /// Does not show times that have a confirmedReservation
        /// TODO: support date ranges instead of just a single date
        /// </summary>
        /// <param name="providerId">Use a providerId where you have already use the CreateTimeSlots</param>
        /// <param name="date">The date portion only is fine '2024-01-12'</param>
        /// <returns></returns>
        [HttpGet(nameof(GetAvailableTimeSlots))]
        public IEnumerable<TimeSlot> GetAvailableTimeSlots(int providerId, DateTimeOffset date)
        {
            return _providerService.GetAvailableTimeSlots(providerId, date);
        }

        /// <summary>
        /// Use the timeSlotId from GetAvailableTimeSlots.  Reservations must be made more than 24 hours in advance.
        /// </summary>
        /// <param name="timeSlotId">Retrieve this from GetAvailableTimeSlots</param>
        /// <param name="clientId">Any integer works. TODO: implement new controller Client Create</param>
        /// <returns></returns>
        //TODO remove the client ID and automatically pull the clientID from the authorization Token credentials
        [HttpPatch(nameof(ReserveTimeSlot))]
        public bool ReserveTimeSlot(Guid timeSlotId, int clientId)
        {
            return _providerService.ReserveTimeSlot(timeSlotId, clientId);
        }

        /// <summary>
        /// Confirms the reservation.  Note that the clintId must match the clientId that reserved the timeSlot in the first place.
        /// </summary>
        /// <param name="timeSlotId">Retrieve this from GetAvailableTimeSlots</param>
        /// <param name="clientId">Any integer works. TODO: implement new controller Client Create</param>
        /// <returns></returns>
        //TODO remove the client ID and automatically pull the clientID from the authorization Token credentials
        [HttpPatch(nameof(ConfirmReservation))]
        public bool ConfirmReservation(Guid timeSlotId, int clientId)
        {
            return _providerService.ConfirmReservation(timeSlotId, clientId);
        }
    }
}