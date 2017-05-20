using System;
using System.Collections.Generic;
using System.Linq;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.MessageProcessors
{
    public class MessageProcessorFactory : IMessageProcessorFactory
    {
        private readonly IReadOnlyCollection<IMessageProcessor> _messageProcessors;

        public MessageProcessorFactory(IMissionMessageProcessor missionMessageProcessor)
        {
            _messageProcessors = new List<IMessageProcessor>
            {
                missionMessageProcessor
            };
        }

        public IMessageProcessor GetMessageProcessor(Message message)
        {
            var messageProcessor = _messageProcessors.FirstOrDefault(ep => ep.CanProcess(message));
            if (messageProcessor == null)
            {
                throw new NotSupportedException($"Can't find MessageProcessor for \"{message.MessageType}\" message type");
            }
            return messageProcessor;
        }
    }
}