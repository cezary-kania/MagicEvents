using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Repositories;

namespace MagicEvents.Api.Service.Infrastructure.Repositories.InMemoryRepositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private ISet<User> _users = new HashSet<User>();
        
        public Task AddAsync(User user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid userId)
        {
            var user = _users.SingleOrDefault(x => x.Id == userId);
            _users.Remove(user);
            return Task.CompletedTask;
        }

        public Task<User> GetAsync(Guid userId)
        {
            var user = _users.SingleOrDefault(x => x.Id == userId);
            return Task.FromResult(user);
        }

        public Task<User> GetAsync(string userEmail)
        {
            var user = _users.SingleOrDefault(x => x.Identity.Email == userEmail);
            return Task.FromResult(user);
        }

        public Task UpdateAsync(User user)
        {
            var existingUser = Task.Run(async () => await GetAsync(user.Id)).Result;
            _users.Remove(existingUser);
            _users.Add(user);
            return Task.CompletedTask;
        }
    }
}