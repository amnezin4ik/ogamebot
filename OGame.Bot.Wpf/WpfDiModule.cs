using Autofac;
using AutoMapper;
using Caliburn.Micro;
using OGame.Bot.Domain.Services;
using OGame.Bot.Wpf.Services;

namespace OGame.Bot.Wpf
{
    public class WpfDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterAutomapperConfiguration(builder);
            builder.RegisterType<NavigationService>().As<Services.INavigationService>().SingleInstance();
        }

        private void RegisterAutomapperConfiguration(ContainerBuilder builder)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainMappingProfile());
            });
            var mapper = config.CreateMapper();
            builder.RegisterInstance(mapper).As<IMapper>();
        }
    }
}