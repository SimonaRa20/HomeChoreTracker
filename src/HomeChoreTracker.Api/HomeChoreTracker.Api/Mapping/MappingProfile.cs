using AutoMapper;
using HomeChoreTracker.Api.Contracts.Auth;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegisterRequest, User>();
            CreateMap<User, UserRegisterResponse>();
        }
    }
}
