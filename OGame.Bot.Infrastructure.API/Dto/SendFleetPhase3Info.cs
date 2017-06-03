using System.Collections.Generic;

namespace OGame.Bot.Infrastructure.API.Dto
{
    public class SendFleetPhase3Info
    {
        public SendFleetPhase3Info()
        {
            AlreadySetedParameters = new Dictionary<string, string>();
        }

        public Dictionary<string, string> AlreadySetedParameters { get; set; }

        public double FleetLoadRoomCapacity { get; set; }
    }
}