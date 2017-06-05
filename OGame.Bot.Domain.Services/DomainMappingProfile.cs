using AutoMapper;
using Dto = OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Domain.Services
{
    public class DomainMappingProfile : Profile
    {
        public DomainMappingProfile()
        {
            CreateMap<Dto.Coordinates, Coordinates>();
            CreateMap<SessionData, Dto.SessionData>();
            CreateMap<Dto.SessionData, SessionData>();
            CreateMap<Dto.Mission, Mission>();
            CreateMap<UserPlanet, MissionPlanet>();
            CreateMap<Dto.UserPlanet, UserPlanet>();
            CreateMap<Dto.MissionPlanet, MissionPlanet>();
            CreateMap<MissionPlanet, Dto.MissionPlanet>();
        }
    }
}