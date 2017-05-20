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

        public async Task Process(Message message)
        {
            throw new System.NotImplementedException();
        }

        public bool ShouldProcessRightNow(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}
