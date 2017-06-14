using System.Collections.Generic;
using OGame.Bot.Domain.Messages;
using OGame.Bot.Domain.Services.MessageProcessors.Interfaces;

namespace OGame.Bot.Domain.Services.MessageProcessors
{
    public interface IMessageProcessorFactory
    {
        IEnumerable<IMessageProcessor> GetMessageProcessors(Message message);
    }
}