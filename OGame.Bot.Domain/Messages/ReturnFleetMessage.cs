using System;

namespace OGame.Bot.Domain.Messages
{
    public class ReturnFleetMessage : Message
    {
        public ReturnFleetMessage(FleetMovement saveMovement, TimeSpan approximateStartOfReturn) 
            : base(MessageType.ReturnFleet)
        {
            SaveMovement = saveMovement;
            ApproximateStartOfReturn = approximateStartOfReturn;
        }

        public FleetMovement SaveMovement { get; }

        public TimeSpan ApproximateStartOfReturn { get; }

        public override int GetHashCode()
        {
            return SaveMovement.Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{MessageType}. SaveMovement: {SaveMovement}. ApproximateStartOfReturn {ApproximateStartOfReturn}";
        }
    }
}