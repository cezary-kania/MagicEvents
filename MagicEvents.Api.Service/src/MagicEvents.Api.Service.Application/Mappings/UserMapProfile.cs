using AutoMapper;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.ValueObjects;

namespace MagicEvents.Api.Service.Application.Mappings
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