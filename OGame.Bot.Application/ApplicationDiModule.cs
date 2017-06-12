using Autofac;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Application.Services;
using OGame.Bot.Domain.Messages;
using OGame.Bot.Domain.Services.MessageProcessors;
using OGame.Bot.Domain.Services.MessageProcessors.Implementations;
using OGame.Bot.Domain.Services.MessageProcessors.Interfaces;

namespace OGame.Bot.Application
{
    public class ApplicationDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BotService>().As<IBotService>();
            builder.RegisterType<GlobalStateUpdater>().As<IGlobalStateUpdater>().SingleInstance();
            builder.RegisterType<MessageServiceBus>().As<IMessageServiceBus>().SingleInstance();
            builder.RegisterType<MessagesComparer>().As<IMessagesComparer>();
            RegisterMessageProcessors(builder);
        }

        private void RegisterMessageProcessors(ContainerBuilder builder)
        {
            builder.RegisterType<MessageProcessorFactory>().As<IMessageProcessorFactory>();

            builder.RegisterType<AttackMessageProcessor>().As<IAttackMessageProcessor>();
            builder.RegisterType<UpdateSessionDataMessageProcessor>().As<IUpdateSessionDataMessageProcessor>();
            builder.RegisterType<ReturnFleetMessageProcessor>().As<IReturnFleetMessageProcessor>();
            builder.RegisterType<FleetArrivedMessageProcessor>().As<IFleetArrivedMessageProcessor>();
            builder.RegisterType<UpdateStateMessageProcessor>().As<IUpdateStateMessageProcessor>();
            builder.RegisterType<TransportMessageProcessor>().As<ITransportMessageProcessor>();
        }
    }
}