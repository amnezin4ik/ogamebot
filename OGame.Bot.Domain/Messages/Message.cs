namespace OGame.Bot.Domain.Messages
{
    public abstract class Message
    {
        protected Message(MessageType messageType)
        {
            MessageType = messageType;
        }

        public MessageType MessageType { get; }

        public abstract override int GetHashCode();

        public abstract override string ToString();
    }
}