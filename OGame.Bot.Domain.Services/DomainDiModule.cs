using Autofac;
using OGame.Bot.Domain.Services.Implementations;
using OGame.Bot.Domain.Services.Interfaces;

namespace OGame.Bot.Domain.Services
{
    public class DomainDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MissionService>().As<IMissionService>();
            builder.RegisterType<GalaxyService>().As<IGalaxyService>();
            builder.RegisterType<FleetService>().As<IFleetService>();
            builder.RegisterType<UpdatableSessionDataProvider>().As<IUpdatableSessionDataProvider>();
            builder.RegisterType<UpdatableSessionDataProvider>().As<ISessionDataProvider>();
            builder.RegisterType<UserPlanetsService>().As<IUserPlanetsService>();
        }
    }
}