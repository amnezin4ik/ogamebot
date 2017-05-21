using System;

namespace OGame.Bot.Domain
{
    public class Mission
    {
        public Mission(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public TimeSpan ArrivalTimeUtc { get; set; }

        public MissionType MissionType { get; set; }

        public MissionPlanet PlanetFrom { get; set; }

        public MissionPlanet PlanetTo { get; set; }
    }
}
