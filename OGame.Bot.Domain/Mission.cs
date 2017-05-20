﻿using System;

namespace OGame.Bot.Domain
{
    public class Mission
    {
        public string Id { get; set; }

        public TimeSpan ArrivalTimeUtc { get; set; }

        public MissionType MissionType { get; set; }

        public MissionPlanet PlanetFrom { get; set; }

        public MissionPlanet PlanetTo { get; set; }
    }
}
