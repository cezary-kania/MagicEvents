using System;
using MagicEvents.Api.Service.Application.DTOs.Events.CreateEvent;

namespace MagicEvents.Api.Service.IntrationTests.DataFactories
{
    public static class EventTestDataFactory
    {
        public static CreateEventDto CreateTestEventDto() 
        {
            Guid randomUnique = Guid.NewGuid();
            Random rand = new Random();
            return new CreateEventDto 
            {
                Title = $"Test event - {randomUnique}",
                Description = $"Test event description - {randomUnique}",
                StartsAt = DateTime.UtcNow.AddDays(rand.Next(3,6)),
                EndsAt = DateTime.UtcNow.AddDays(rand.Next(7,10))
            };
        }
    }
}