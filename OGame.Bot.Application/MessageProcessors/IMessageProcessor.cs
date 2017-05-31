using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.MessageProcessors
{
    public interface IMessageProcessor
    {
        bool CanProcess(Message message);

        bool ShouldProcessRightNow(Message message);

        Task<IEnumerable<Message>> ProcessAsync(Message message);
    }
}