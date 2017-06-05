using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain.Services.Interfaces;

namespace OGame.Bot.Application.MessageProcessors
{
    public class UpdateSessionDataMessageProcessor : IUpdateSessionDataMessageProcessor
    {
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