using System;

namespace OGame.Bot.Infrastructure.API.Dto
{
    public class FleetEvent
    {
        public string Id { get; set; }

        public TimeSpan ArrivalTimeUtc { get; set; }

        public MissionType MissionType { get; set; }

        public FleetEventPlanet PlanetFrom { get; set; }

        public FleetEventPlanet PlanetTo { get; set; }
    }
}
