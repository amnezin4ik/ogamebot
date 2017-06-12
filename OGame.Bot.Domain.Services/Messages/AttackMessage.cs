using System;

namespace OGame.Bot.Domain.Services.Messages
{
    public sealed class AttackMessage : Message
    {
        private readonly Mission _attackMission;

        public AttackMessage(Mission attackMission) 
            : base(MessageType.Attack)
        {
            _attackMission = attackMission;
        }

        public string MissionId => _attackMission.Id;

        public TimeSpan ArrivalTimeUtc => _attackMission.ArrivalTimeUtc;

        public MissionPlanet PlanetFrom => _attackMission.PlanetFrom;

        public MissionPlanet PlanetTo => _attackMission.PlanetTo;

        public override int GetHashCode()
        {
            return _attackMission.Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{MessageType}. MissionId: {MissionId}, {PlanetFrom} -> {PlanetTo}. ArrivalTimeUtc {ArrivalTimeUtc}";
        }
    }
}