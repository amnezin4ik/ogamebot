using System.Threading;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Application.Services;

namespace OGame.Bot.Wpf
{
    public class ApplicationUsageSample
    {
        private readonly IMessageServiceBus _messageServiceBus;
        private readonly IGlobalStateUpdater _globalStateUpdater;

        public ApplicationUsageSample(IMessageServiceBus messageServiceBus, IGlobalStateUpdater globalStateUpdater)
        {
            _messageServiceBus = messageServiceBus;
            _globalStateUpdater = globalStateUpdater;
        }

        public void Run(CancellationToken cancellationToken)
        {
            //TODO: I think it should be done otherwise, but right now I don't know how XD
            _messageServiceBus.Run();
            _globalStateUpdater.Run();
        }
    }
}