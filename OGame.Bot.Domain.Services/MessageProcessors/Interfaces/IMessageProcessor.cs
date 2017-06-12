using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Domain.Services.Messages;

namespace OGame.Bot.Domain.Services.MessageProcessors.Interfaces
{
    public interface IMessageProcessor
    {
        bool CanProcess(Message message);

        bool ShouldProcessRightNow(Message message);

        Task<IEnumerable<Message>> ProcessAsync(Message message);
    }
}