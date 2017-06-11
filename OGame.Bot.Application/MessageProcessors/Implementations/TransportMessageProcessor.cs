﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Application.MessageProcessors.Interfaces;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.MessageProcessors.Implementations
{
    public class TransportMessageProcessor : ITransportMessageProcessor
    {
        private readonly Logger _logger = LogManager.GetLogger(nameof(TransportMessageProcessor));

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