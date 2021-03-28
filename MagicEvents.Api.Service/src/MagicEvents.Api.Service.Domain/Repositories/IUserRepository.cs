using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Domain.Entities;

namespace MagicEvents.Api.Service.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetAsync(Guid userId);
        Task<User> GetAsync(string userEmail);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid userId);
    }
}