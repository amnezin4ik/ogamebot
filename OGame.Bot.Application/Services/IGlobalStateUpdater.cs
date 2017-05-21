using System.Threading;
using System.Threading.Tasks;
using OGame.Bot.Application.MessageBus;

namespace OGame.Bot.Application.Services
{
    public interface IGlobalStateUpdater
    {
        Task Run(IMessageServiceBus messageServiceBus, CancellationToken cancellationToken);
    }
}