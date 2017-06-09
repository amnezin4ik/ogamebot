using OGame.Bot.Application.MessageProcessors.Interfaces;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.MessageProcessors
{
    public interface IMessageProcessorFactory
    {
        IMessageProcessor GetMessageProcessor(Message message);
    }
}