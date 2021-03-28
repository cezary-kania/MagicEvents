using AutoMapper;
using MagicEvents.CRUD.Service.Application.DTOs.Users;
using MagicEvents.CRUD.Service.Domain.Entities;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Application.Mappings
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<User,UserDto>();
            CreateMap<UserEventActivity,UserEventActivityDto>();
            CreateMap<UserIdentity,UserIdentityDto>();
            CreateMap<UserProfile,UserProfileDto>();
            CreateMap<UserProfile,UserProfileBaseDto>();
        }
    }
}