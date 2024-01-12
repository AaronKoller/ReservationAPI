using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using ReservationAPIBonusRound;
using ReservationAPIBonusRound.Models;
using ReservationAPIBonusRound.Models.AppSettings;

namespace ReservationAPIBonusRoundTests
{
    public class IndexPageTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly AppSettingsOptions _defaultAppSettings;

        public IndexPageTests(CustomWebApplicationFactory<Program> factory)
        {
            _defaultAppSettings = new AppSettingsOptions
            {
                ReservationTimeoutInSeconds = 1800,
                MinimumTimeBlockInSeconds = 900,
                MinimumAdvancedReservationInDays = 1,
            };

            _factory = factory;
            _factory.appSettings = _defaultAppSettings;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        //This single in-memory integration test achieves 93% code coverage for all happy path code flow.

        [Fact]
        public async Task HappyPath_ShouldCreateSlotsAndConfirmReservationForASingleClientAndProvider()
        {
            var providerId = 1;
            var clientId = 1;
            var startTime = new DateTimeOffset(2024, 1, 22, 14, 0, 0, 0, TimeSpan.FromHours(0));
            var endTime = new DateTimeOffset(2024, 1, 22, 15, 0, 0, 0, TimeSpan.FromHours(0));
            var totalSlots = Convert.ToInt32((endTime - startTime).TotalMinutes / (_defaultAppSettings.MinimumTimeBlockInSeconds / 60));

            //Create time slots with ProviderId
            var startTimeString = startTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var endTimeString = endTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var createTimeSlotsResponse = await _client.PostAsync($"/Provider/CreateTimeSlots?providerId={providerId}&startTime={startTimeString}&endTime={endTimeString}", null);
            createTimeSlotsResponse.IsSuccessStatusCode.Should().BeTrue();
            var createTimeSlotContent = await createTimeSlotsResponse.Content.ReadAsStringAsync();
            JsonConvert.DeserializeObject<bool>(createTimeSlotContent).Should().BeTrue();

            //Get all timeSlots for providerId
            var getTimeSlotsResponse = await _client.GetAsync($"/Provider/GetAvailableTimeSlots?providerId={providerId}&date={startTimeString}");
            getTimeSlotsResponse.IsSuccessStatusCode.Should().BeTrue();
            var getTimeSlotsContent = await getTimeSlotsResponse.Content.ReadAsStringAsync();
            var timeSlots = JsonConvert.DeserializeObject<List<TimeSlot>>(getTimeSlotsContent);
            timeSlots.Should().NotBeNull();
            timeSlots.Count.Should().Be(totalSlots);

            //create a reservation
            var timeSlot = timeSlots.First();
            var reserveTimeSlotResponse = await _client.PatchAsync($"/Provider/ReserveTimeSlot?timeSlotId={timeSlot.TimeSlotId}&clientId={clientId}", null);
            reserveTimeSlotResponse.IsSuccessStatusCode.Should().BeTrue();
            var reserveTimeSlotContent = await reserveTimeSlotResponse.Content.ReadAsStringAsync();
            JsonConvert.DeserializeObject<bool>(reserveTimeSlotContent).Should().BeTrue();

            //We should have one time slot missing as it is reserved
            var getTimeSlotsResponse2 = await _client.GetAsync($"/Provider/GetAvailableTimeSlots?providerId={providerId}&date={startTimeString}");
            var getTimeSlotsContent2 = await getTimeSlotsResponse2.Content.ReadAsStringAsync();
            var timeSlots2 = JsonConvert.DeserializeObject<List<TimeSlot>>(getTimeSlotsContent2);
            timeSlots2.Count.Should().Be(totalSlots - 1);

            //We should be able to confirm this reservation this time
            var confirmReservationResponse = await _client.PatchAsync($"/Provider/ConfirmReservation?timeSlotId={timeSlot.TimeSlotId}&clientId={clientId}", null);
            confirmReservationResponse.IsSuccessStatusCode.Should().BeTrue();
            var confirmReservationContent = await confirmReservationResponse.Content.ReadAsStringAsync();
            JsonConvert.DeserializeObject<bool>(confirmReservationContent).Should().BeTrue();
        }
    }
}