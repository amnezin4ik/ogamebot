namespace OGame.Bot.Application.Messages
{
    public sealed class UpdateSessionDataMessage : Message
    {
        public UpdateSessionDataMessage() 
            : base(MessageType.UpdateSessionData)
        {
        }

        public override int GetHashCode()
        {
            return typeof(UpdateSessionDataMessage).GetHashCode();
        }
    }
}