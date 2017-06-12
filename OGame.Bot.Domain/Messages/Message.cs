namespace OGame.Bot.Domain.Messages
{
    public abstract class Message
    {
        protected Message(MessageType messageType)
        {
            MessageType = messageType;
        }

        public MessageType MessageType { get; }

        //TODO: Maybe implement another method like "string GetMessageKey()" instead of GetHashCode(), and make messages compare by compare message keys
        public abstract override int GetHashCode();

        public abstract override string ToString();
    }
}