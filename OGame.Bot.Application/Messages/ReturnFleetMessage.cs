using OGame.Bot.Domain;

namespace OGame.Bot.Application.Messages
{
    public class ReturnFleetMessage : Message
    {
        private readonly Mission _saveMission;

        public ReturnFleetMessage(Mission saveMission) 
            : base(MessageType.ReturnFleet)
        {
            _saveMission = saveMission;
        }

        public override int GetHashCode()
        {
            return _saveMission.Id.GetHashCode();
        }
    }
}