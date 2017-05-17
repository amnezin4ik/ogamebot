using System;

namespace OGame.Bot.Infrastructure.API.Dto
{
    public class FleetEvent
    {
        public string Id { get; set; }

        public TimeSpan ArrivalTimeUtc { get; set; }

        public FleetMissionType FleetMissionType { get; set; }

        public FleetEventPlanet PlanetFrom { get; set; }

        public FleetEventPlanet PlanetTo { get; set; }
    }
}
