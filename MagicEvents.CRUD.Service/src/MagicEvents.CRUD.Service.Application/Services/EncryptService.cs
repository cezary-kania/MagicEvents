using System;
using System.Security.Cryptography;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using MagicEvents.CRUD.Service.Domain.Entities;

namespace MagicEvents.CRUD.Service.Application.Services
{
    // Implementation of this service is based on Piotr Gankiewicz (github: spetz) examples
    public class EncryptService : IEncryptService
    {
        private static readonly int _saltSize = 40;
        private static readonly int _iterations = 1000;

        public string GenerateSalt()
        {
            var saltBytes = new byte[_saltSize];
            new RNGCryptoServiceProvider().GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public string GetHash(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, _iterations);
            return Convert.ToBase64String(pbkdf2.GetBytes(_saltSize));
        }

        public bool ValidatePassword(User user, string passwordToVerify)
        {
            var newHash = GetHash(passwordToVerify, user.Identity.Salt);
            return newHash == user.Identity.Password;
        }
    }
}