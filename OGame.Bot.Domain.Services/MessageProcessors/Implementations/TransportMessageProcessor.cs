using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Domain.Messages;
using OGame.Bot.Domain.Services.MessageProcessors.Interfaces;

namespace OGame.Bot.Domain.Services.MessageProcessors.Implementations
{
    public class TransportMessageProcessor : ITransportMessageProcessor
    {
        private readonly ILogger _logger = LogManager.GetLogger(nameof(TransportMessageProcessor));

        public bool CanProcess(Message message)
        {
            return message.MessageType == MessageType.Transport;
        }

        public bool ShouldProcessRightNow(Message message)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Message>> ProcessAsync(Message message)
        {
            throw new NotImplementedException();
        }
    }
}