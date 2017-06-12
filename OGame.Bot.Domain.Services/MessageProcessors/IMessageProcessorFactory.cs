using OGame.Bot.Domain.Services.MessageProcessors.Interfaces;
using OGame.Bot.Domain.Services.Messages;

namespace OGame.Bot.Domain.Services.MessageProcessors
{
    public interface IMessageProcessorFactory
    {
        IMessageProcessor GetMessageProcessor(Message message);
    }
}