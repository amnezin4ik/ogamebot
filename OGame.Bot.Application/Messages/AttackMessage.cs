namespace OGame.Bot.Application.Messages
{
    public sealed class AttackMessage : Message
    {
        public AttackMessage(MessageType messageType) : base(messageType)
        {
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}
