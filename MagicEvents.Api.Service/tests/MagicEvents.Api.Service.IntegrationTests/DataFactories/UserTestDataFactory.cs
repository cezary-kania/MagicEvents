using System;
using System.Collections.Generic;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.RegisterUser;

namespace MagicEvents.Api.Service.IntegrationTests.DataFactories
{
    public static class UserTestDataFactory
    {
        public static List<RegisterUserDto> CreateTestUsers(int userNumbers)
        {
            List<RegisterUserDto> newUsers = new List<RegisterUserDto>();
            for(int i = 0; i < userNumbers; ++i)
            {
                newUsers.Add(CreateTestUser());
            }
            return newUsers;
        }

        public static RegisterUserDto CreateTestUser()
        {
            Guid randomUnique = Guid.NewGuid();
            return new RegisterUserDto 
            {
                Email = $"{randomUnique}@test.com",
                Password = $"P4SSw0rd-{randomUnique}"
            };
        }
    }
}