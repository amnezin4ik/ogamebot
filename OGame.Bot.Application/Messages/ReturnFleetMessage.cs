using System;
using OGame.Bot.Domain;

namespace OGame.Bot.Application.Messages
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
    }
}