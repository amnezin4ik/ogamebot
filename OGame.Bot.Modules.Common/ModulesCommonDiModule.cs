using Autofac;

namespace OGame.Bot.Modules.Common
{
    public class ModulesCommonDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SystemDateTimeProvider>().As<IDateTimeProvider>();
        }
    }
}