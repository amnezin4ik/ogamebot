using AutoMapper;

namespace OGame.Bot.SimpleWpf
{
    public class WpfMappingProfile : Profile
    {
        public WpfMappingProfile()
        {
            CreateMap<Models.UserCredentials, Application.Models.UserCredentials>();
        }
    }
}