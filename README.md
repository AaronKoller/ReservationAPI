# ReservationAPI
All APIs are lightly documented for the reviewers sake and does not reflect how this would be documented in a production environment.  There are a few //TODOs where I call out instances that I would like to have changed to reflect a more production-ready application.  This was built with Visual Studio 2022 and is utilizing .Net Core 6.0.

### Usage. 
Open in visual studio 2022 and run application.

1. To create a schedule call for ProviderId 1 from 8am to 4pm  
**POST**: https://localhost:7211/Provider/CreateTimeSlots?providerId=1&startTime=2024-01-12T08:00:00.000Z&endTime=2024-01-12T16:00:00.000Z

3. Get a list of all schedules created on that same day by the same provider  
**GET**:  https://localhost:7211/Provider/GetAvailableTimeSlots?providerId=1&date=2024-01-12
Returns an array of available time slots
```
[
    {
        "isReservationConfirmed": false,
        "timeSlotId": "63ddd473-fe1d-4920-afdd-2a6942c90d4b",
        "starTime": "2024-01-12T08:00:00+00:00",
        "endTime": "2024-01-12T08:15:00+00:00",
        "reservedTime": "0001-01-01T00:00:00+00:00",
        "reservedByClientId": 0
    },
    ...
]
```

3. To make a reservation use a TimeSlotID such as '63ddd473-fe1d-4920-afdd-2a6942c90d4b'.  Note that the guid will be different on your machine as the guids are random.  Assign this to client ID of 1  
**PATCH**: https://localhost:7211/Provider/ReserveTimeSlot?timeSlotId=63ddd473-fe1d-4920-afdd-2a6942c90d4b&clientId=1

4. To Confirm the reservation call with the same client ID and the same GUID  
**PATCH**: https://localhost:7211/Provider/ConfirmReservation?timeSlotId=63ddd473-fe1d-4920-afdd-2a6942c90d4b&clientId=1

# ReservationApiBonusRound (Extra Solution)   
This was done for my benefit but I figured I would include it as this was one outside the 3 hour time limit.  There is a single Integration Test with 93% code coverage for all happy paths.  This integration test starts a real kestrel server and then the test then proceeds to hit this in-memory endpoints using the service as though it is live and running.  As a consequence we have a fast test that runs through all happy path scenarios in .250 seconds.  While I had done this in .net core 3.1 there are significant changes in .net core 6 and I wanted to see what those changes are.  One major benefit is that I can create appSettings in the test which would allow me to test many scenarios.
