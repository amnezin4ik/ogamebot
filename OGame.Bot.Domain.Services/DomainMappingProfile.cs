using AutoMapper;
using Dto = OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Domain.Services
{
    public class DomainMappingProfile : Profile
    {
        public DomainMappingProfile()
        {
            CreateMap<Dto.Coordinates, Coordinates>();
        }
    }
}