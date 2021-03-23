using AutoMapper;
using MagicEvents.CRUD.Service.Application.DTOs.Users;
using MagicEvents.CRUD.Service.Domain.Entities;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User,UserDto>();
            CreateMap<UserEventActivity,UserEventActivityDto>();
            CreateMap<UserIdentity,UserIdentityDto>();
            CreateMap<UserProfile,UserProfileDto>();
        }
    }
}