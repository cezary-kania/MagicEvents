using MagicEvents.Api.Service.Domain.Entities;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IEncryptService
    {
        string GetHash(string password, string salt);
        bool ValidatePassword(User user, string passwordToVerify);
        string GenerateSalt();
    }
}