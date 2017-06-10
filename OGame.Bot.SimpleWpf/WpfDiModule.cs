using Autofac;
using AutoMapper;
using OGame.Bot.Domain.Services;

namespace OGame.Bot.SimpleWpf
{
    public class WpfDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterAutomapperConfiguration(builder);
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