using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Repositories;
using MagicEvents.Api.Service.Infrastructure.MongoDb.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicEvents.Api.Service.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        public UserRepository(IMongoDbSettings mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            var db = client.GetDatabase(mongoDbSettings.DatabaseName);
            _users = db.GetCollection<User>(mongoDbSettings.UsersCollectionName);
        }
        public async Task AddAsync(User user)
            => await _users.InsertOneAsync(user);

        public async Task DeleteAsync(Guid userId)
            => await _users.DeleteOneAsync(x => x.Id == userId);

        public async Task<User> GetAsync(Guid userId)
            => await _users.AsQueryable().FirstOrDefaultAsync(x => x.Id == userId);

        public async Task<User> GetAsync(string userEmail)
            => await _users.AsQueryable().FirstOrDefaultAsync(x => x.Identity.Email == userEmail);

        public async Task UpdateAsync(User user)
            => await _users.ReplaceOneAsync(x => x.Id == user.Id, user);
    }
}