using AutoMapper;
using OGame.Bot.Application.Models;

namespace OGame.Bot.Application
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<UserCredentials, Domain.UserCredentials>();
        }
    }
}