using AutoMapper;
using OGame.Bot.Wpf.Models;

namespace OGame.Bot.Wpf
{
    public class WpfMappingProfile : Profile
    {
        public WpfMappingProfile()
        {
            CreateMap<UserCredentials, Application.Models.UserCredentials>();
        }
    }
}