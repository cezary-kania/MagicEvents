using MagicEvents.CRUD.Service.Domain.Entities;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IEncryptService
    {
        string GetHash(string password, string salt);
        bool ValidatePassword(User user, string passwordToVerify);
        string GenerateSalt();
    }
}