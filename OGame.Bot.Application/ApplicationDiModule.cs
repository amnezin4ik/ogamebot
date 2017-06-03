using Autofac;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Application.Services;

namespace OGame.Bot.Application
{
    public class ApplicationDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GlobalStateUpdater>().As<IGlobalStateUpdater>().SingleInstance();
            builder.RegisterType<MessageServiceBus>().As<IMessageServiceBus>().SingleInstance();
            builder.RegisterType<MessagesProvider>().As<IMessagesProvider>();
        }
    }
}