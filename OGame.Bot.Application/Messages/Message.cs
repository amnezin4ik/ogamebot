namespace OGame.Bot.Application.Messages
{
    public abstract class Message
    {
        protected Message(MessageType messageType)
        {
            MessageType = messageType;
        }

        public MessageType MessageType { get; }
    }
}
