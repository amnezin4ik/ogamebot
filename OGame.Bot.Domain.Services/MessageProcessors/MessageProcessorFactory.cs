using System;
using System.Collections.Generic;
using System.Linq;
using OGame.Bot.Domain.Messages;
using OGame.Bot.Domain.Services.MessageProcessors.Interfaces;

namespace OGame.Bot.Domain.Services.MessageProcessors
{
    public class MessageProcessorFactory : IMessageProcessorFactory
    {
        private readonly IReadOnlyCollection<IMessageProcessor> _messageProcessors;

        public MessageProcessorFactory(IAttackMessageProcessor attackMessageProcessor, 
                                       IUpdateSessionDataMessageProcessor updateSessionDataMessageProcessor,
                                       IReturnFleetMessageProcessor returnFleetMessageProcessor,
                                       IFleetArrivedMessageProcessor fleetArrivedMessageProcessor,
                                       IUpdateStateMessageProcessor updateStateMessageProcessor,
                                       ITransportMessageProcessor transportMessageProcessor)
        {
            _messageProcessors = new List<IMessageProcessor>
            {
                attackMessageProcessor,
                updateSessionDataMessageProcessor,
                returnFleetMessageProcessor,
                fleetArrivedMessageProcessor,
                updateStateMessageProcessor,
                transportMessageProcessor
            };
        }

        public IEnumerable<IMessageProcessor> GetMessageProcessors(Message message)
        {
            var messageProcessors = _messageProcessors.Where(mp => mp.CanProcess(message)).ToList();
            if (messageProcessors.Count == 0)
            {
                throw new NotSupportedException($"Can't find MessageProcessor for \"{message.MessageType}\" message type");
            }
            return messageProcessors;
        }
    }
}