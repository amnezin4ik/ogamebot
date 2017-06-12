using System.Collections.Generic;
using System.Text;

namespace OGame.Bot.Domain.Messages
{
    public class UpdateStateMessage : Message
    {
        public UpdateStateMessage(params MessageType [] messageTypesToUpdate) : base(MessageType.UpdateState)
        {
            MessageTypesToUpdate = messageTypesToUpdate;
        }

        public UpdateStateMessage(IEnumerable<MessageType> messageTypesToUpdate) : base(MessageType.UpdateState)
        {
            MessageTypesToUpdate = messageTypesToUpdate;
        }

        public IEnumerable<MessageType> MessageTypesToUpdate { get; }

        public override int GetHashCode()
        {
            var stringBuilder = new StringBuilder(nameof(UpdateStateMessage));
            foreach (var messageType in MessageTypesToUpdate)
            {
                stringBuilder.Append(messageType);
            }
            var hashCode = stringBuilder.ToString().GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{MessageType}. MessageTypesToUpdate: {string.Join(";", MessageTypesToUpdate)}";
        }
    }
}