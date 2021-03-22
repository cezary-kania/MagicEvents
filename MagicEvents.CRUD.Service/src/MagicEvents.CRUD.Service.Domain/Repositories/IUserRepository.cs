using System;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Domain.Entities;

namespace MagicEvents.CRUD.Service.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetAsync(Guid userId);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid userId);
    }
}