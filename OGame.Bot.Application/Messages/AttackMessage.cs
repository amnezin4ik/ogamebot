using System;
using OGame.Bot.Domain;

namespace OGame.Bot.Application.Messages
{
    public sealed class AttackMessage : Message
    {
        private readonly Mission _attackMission;

        public AttackMessage(Mission attackMission) 
            : base(MessageType.Attack)
        {
            _attackMission = attackMission;
        }

        public string Id => _attackMission.Id;

        public TimeSpan ArrivalTimeUtc => _attackMission.ArrivalTimeUtc;

        public MissionPlanet PlanetFrom => _attackMission.PlanetFrom;

        public MissionPlanet PlanetTo => _attackMission.PlanetTo;

        public override int GetHashCode()
        {
            return _attackMission.Id.GetHashCode();
        }
    }
}