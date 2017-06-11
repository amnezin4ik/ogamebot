using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Application.MessageProcessors.Interfaces;
using OGame.Bot.Application.Messages;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.Application.MessageProcessors.Implementations
{
    public class FleetArrivedMessageProcessor : IFleetArrivedMessageProcessor
    {
        private readonly Logger _logger = LogManager.GetLogger(nameof(FleetArrivedMessageProcessor));
        private readonly IDateTimeProvider _dateTimeProvider;

        public FleetArrivedMessageProcessor(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public bool CanProcess(Message message)
        {
            return message.MessageType == MessageType.FleetArrived;
        }

        public bool ShouldProcessRightNow(Message message)
        {
            var fleetArrivedMessage = message as FleetArrivedMessage;
            if (fleetArrivedMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            var utcNow = _dateTimeProvider.GetUtcNow();
            var shouldProcessRightNow = (fleetArrivedMessage.ArrivalTimeUtc - utcNow).TotalSeconds < 0;
            return shouldProcessRightNow;
        }

        public async Task<IEnumerable<Message>> ProcessAsync(Message message)
        {
            var fleetArrivedMessage = message as FleetArrivedMessage;
            if (fleetArrivedMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            _logger.Info("Fleet arrived. Create UpdateStateMessage with MessageType.Attack ");
            var updateStateMessage = new UpdateStateMessage(MessageType.Attack);
            var resultMessages = new List<Message> { updateStateMessage };
            return resultMessages;
        }
    }
}