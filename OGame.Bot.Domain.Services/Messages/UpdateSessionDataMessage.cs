﻿namespace OGame.Bot.Domain.Services.Messages
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

        public override string ToString()
        {
            return $"{MessageType}.";
        }
    }
}