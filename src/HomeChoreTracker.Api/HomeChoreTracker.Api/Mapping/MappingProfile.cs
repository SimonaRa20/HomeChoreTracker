using AutoMapper;
using HomeChoreTracker.Api.Contracts.Auth;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegisterRequest, User>();
            CreateMap<User, UserRegisterResponse>();
            CreateMap<HomeChoreBaseRequest, HomeChoreBase>();
            CreateMap<User, UserGetResponse>();
        }
    }
}
