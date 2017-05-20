using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.MessageProcessors
{
    public class AttackMessageProcessor : IAttackMessageProcessor
    {
        public bool CanProcess(Message message)
        {
            return message.MessageType == MessageType.Attack;
        }

        public async Task<IEnumerable<Message>> ProcessAsync(Message message)
        {
            var attackMessage = message as AttackMessage;
            if (attackMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            throw new NotImplementedException();
        }

        public bool ShouldProcessRightNow(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
