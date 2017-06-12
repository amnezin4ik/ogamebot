using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Domain.Services.MessageProcessors.Interfaces;
using OGame.Bot.Domain.Services.Messages;

namespace OGame.Bot.Domain.Services.MessageProcessors.Implementations
{
    public class UpdateSessionDataMessageProcessor : IUpdateSessionDataMessageProcessor
    {
        private readonly ILogger _logger = LogManager.GetLogger(nameof(UpdateSessionDataMessageProcessor));
        private readonly IUpdatableSessionDataProvider _updatableSessionDataProvider;

        public UpdateSessionDataMessageProcessor(IUpdatableSessionDataProvider updatableSessionDataProvider)
        {
            _updatableSessionDataProvider = updatableSessionDataProvider;
        }

        public bool CanProcess(Message message)
        {
            return message.MessageType == MessageType.UpdateSessionData;
        }

        public bool ShouldProcessRightNow(Message message)
        {
            var isUpdateSessionDataMessage = message is UpdateSessionDataMessage;
            if (!isUpdateSessionDataMessage)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            return true;
        }

        public async Task<IEnumerable<Message>> ProcessAsync(Message message)
        {
            var isUpdateSessionDataMessage = message is UpdateSessionDataMessage;
            if (!isUpdateSessionDataMessage)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            await _updatableSessionDataProvider.RefreshSessionDataAsync();
            return new List<Message>();
        }
    }
}