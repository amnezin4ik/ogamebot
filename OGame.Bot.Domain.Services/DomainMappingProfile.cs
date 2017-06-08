using AutoMapper;
using Dto = OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Domain.Services
{
    public class DomainMappingProfile : Profile
    {
        public DomainMappingProfile()
        {
            CreateMap<Coordinates, Dto.Coordinates>();
            CreateMap<Dto.Coordinates, Coordinates>();
            CreateMap<SessionData, Dto.SessionData>();
            CreateMap<Dto.SessionData, SessionData>();
            CreateMap<Dto.Mission, Mission>();
            CreateMap<UserPlanet, MissionPlanet>();
            CreateMap<Dto.UserPlanet, UserPlanet>();
            CreateMap<UserPlanet, Dto.UserPlanet>();
            CreateMap<Dto.MissionPlanet, MissionPlanet>();
            CreateMap<MissionPlanet, Dto.MissionPlanet>();
            CreateMap<Fleet, Dto.Fleet>();
            CreateMap<Dto.Fleet, Fleet>();
            CreateMap<Ship, Dto.Ship>();
            CreateMap<Dto.Ship, Ship>();
            CreateMap<ShipCell, Dto.ShipCell>();
            CreateMap<Dto.ShipCell, ShipCell>();
            CreateMap<ShipType, Dto.ShipType>();
            CreateMap<Dto.ShipType, ShipType>();
            CreateMap<Resources, Dto.Resources>();
            CreateMap<Dto.Resources, Resources>();
            CreateMap<PlanetOverview, Dto.PlanetOverview>();
            CreateMap<Dto.PlanetOverview, PlanetOverview>();
            CreateMap<Dto.FleetMovement, FleetMovement>();
            CreateMap<FleetMovement, Dto.FleetMovement>();
        }
    }
}