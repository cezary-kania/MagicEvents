using System;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Responses
{
    public class AuthTokenDtoExample : IExamplesProvider<AuthTokenDto>
    {
        public AuthTokenDto GetExamples()
        {
            return new AuthTokenDto
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIyNjJhNTI0Zi1kM2Y3LTQ4MDMtODhjMC0yMzVhOWFkMGE2MDEiLCJqdGkiOiI2NmEwMzgyNi01ZGQ2LTRjMzYtYjdlYy1jNGFhNDJiZGY1MzQiLCJlbWFpbCI6ImV4YW1wbGVAZXhhbXBsZS5jb20iLCJ1bmlxdWVfbmFtZSI6IjI2MmE1MjRmLWQzZjctNDgwMy04OGMwLTIzNWE5YWQwYTYwMSIsIm5iZiI6MTYxNzc0MDc4OCwiZXhwIjoxNjE3NzQxNjg4fQ.CE4dQGzgIpwZZ1Pr4b1tS1JPw7o4wur12bIKbo0qvMU",
                Expiry = DateTime.UtcNow
            };
        }
    }
}