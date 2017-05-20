using System.Threading.Tasks;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.MessageProcessors
{
    public interface IMessageProcessor
    {
        bool CanProcess(Message message);

        Task ProcessAsync(Message message);

        bool ShouldProcessRightNow(Message message);
    }
}