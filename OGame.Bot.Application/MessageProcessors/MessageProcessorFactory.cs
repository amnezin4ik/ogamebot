using System;
using System.Collections.Generic;
using System.Linq;
using OGame.Bot.Application.MessageProcessors.Interfaces;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.MessageProcessors
{
    public class MessageProcessorFactory : IMessageProcessorFactory
    {
        private readonly IReadOnlyCollection<IMessageProcessor> _messageProcessors;

        public MessageProcessorFactory(IAttackMessageProcessor attackMessageProcessor, 
                                       IUpdateSessionDataMessageProcessor updateSessionDataMessageProcessor,
                                       IReturnFleetMessageProcessor returnFleetMessageProcessor,
                                       IFleetArrivedMessageProcessor fleetArrivedMessageProcessor,
                                       IUpdateStateMessageProcessor updateStateMessageProcessor)
        {
            _messageProcessors = new List<IMessageProcessor>
            {
                attackMessageProcessor,
                updateSessionDataMessageProcessor,
                returnFleetMessageProcessor,
                fleetArrivedMessageProcessor,
                updateStateMessageProcessor
            };
        }

        public IMessageProcessor GetMessageProcessor(Message message)
        {
            var messageProcessor = _messageProcessors.FirstOrDefault(mp => mp.CanProcess(message));
            if (messageProcessor == null)
            {
                throw new NotSupportedException($"Can't find MessageProcessor for \"{message.MessageType}\" message type");
            }
            return messageProcessor;
        }
    }
}