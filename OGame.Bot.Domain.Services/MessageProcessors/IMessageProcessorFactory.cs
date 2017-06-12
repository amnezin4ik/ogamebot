using OGame.Bot.Domain.Messages;
using OGame.Bot.Domain.Services.MessageProcessors.Interfaces;

namespace OGame.Bot.Domain.Services.MessageProcessors
{
    public interface IMessageProcessorFactory
    {
        IMessageProcessor GetMessageProcessor(Message message);
    }
}