using System;

namespace OGame.Bot.Domain.Messages
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

        public override string ToString()
        {
            return $"{MessageType}. ArrivalTimeUtc: {ArrivalTimeUtc}";
        }
    }
}