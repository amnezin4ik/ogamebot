using System.Threading;
using System.Threading.Tasks;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.MessageBus
{
    public interface IMessageServiceBus
    {
        void EnqueueMessage(Message message);
        Task Run(CancellationToken cancellationToken);
    }
}