using System;

namespace OGame.Bot.Application.Messages
{
    public class FleetArrivedMessage : Message
    {
        public FleetArrivedMessage(TimeSpan arrivalTimeUtc) : base(MessageType.FleetArrived)
        {
            ArrivalTimeUtc = arrivalTimeUtc;
        }

        public TimeSpan ArrivalTimeUtc { get; }

        public override int GetHashCode()
        {
            return ArrivalTimeUtc.GetHashCode();
        }
    }
}