using System.Collections.Generic;

namespace OGame.Bot.Infrastructure.API.Dto
{
    public class GoPhaseInfo
    {
        public GoPhaseInfo()
        {
            AlreadySetedParameters = new Dictionary<string, string>();
        }

        public Dictionary<string, string> AlreadySetedParameters { get; set; }

        public double FleetLoadRoomCapacity { get; set; }
    }
}