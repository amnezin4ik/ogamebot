using System;
using System.Text;

namespace OGame.Bot.Domain
{
    public class FleetMovement
    {
        public string Id { get; set; }

        public TimeSpan ArrivalTimeUtc { get; set; }

        public TimeSpan ReturnTimeUtc { get; set; }

        public MissionType MissionType { get; set; }

        public MissionPlanet PlanetFrom { get; set; }

        public MissionPlanet PlanetTo { get; set; }

        public bool IsReturn { get; set; }

        public TimeSpan GetTimeToReturn(TimeSpan utcNow)
        {
            var oneWayTime = ReturnTimeUtc - ArrivalTimeUtc;
            var timeToArrival = ArrivalTimeUtc - utcNow;
            var timeToReturn = oneWayTime - timeToArrival;
            return timeToReturn;
        }

        public override string ToString()
        {
            var result = $"Id:{Id}, {PlanetFrom} -> {PlanetTo}. ArrivalTimeUtc:({ArrivalTimeUtc}), {MissionType}. Return time: ReturnTimeUtc:{ReturnTimeUtc}, is return: {IsReturn}";
            return result;
        }
    }
}