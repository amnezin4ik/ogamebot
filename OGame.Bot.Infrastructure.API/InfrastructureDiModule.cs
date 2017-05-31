using Autofac;
using OGame.Bot.Infrastructure.API.APIClients;
using OGame.Bot.Infrastructure.API.Helpers;

namespace OGame.Bot.Infrastructure.API
{
    public class InfrastructureDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpHelper>().As<IHttpHelper>().SingleInstance();
            builder.RegisterType<AuthorizationClient>().As<IAuthorizationClient>();
            builder.RegisterType<FleetClient>().As<IFleetClient>();
            builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
            builder.RegisterType<MissionClient>().As<IMissionClient>();
            builder.RegisterType<ResourcesClient>().As<IResourcesClient>();
        }
    }
}