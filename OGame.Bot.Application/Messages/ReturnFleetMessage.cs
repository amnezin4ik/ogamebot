using System;
using OGame.Bot.Domain;

namespace OGame.Bot.Application.Messages
{
    public class ReturnFleetMessage : Message
    {
        public ReturnFleetMessage(Mission saveMission, TimeSpan approximateStartOfReturn) 
            : base(MessageType.ReturnFleet)
        {
            SaveMission = saveMission;
            ApproximateStartOfReturn = approximateStartOfReturn;
        }

        public Mission SaveMission { get; }

        public TimeSpan ApproximateStartOfReturn { get; }

        public override int GetHashCode()
        {
            return SaveMission.Id.GetHashCode();
        }
    }
}