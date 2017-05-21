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

        public string Id
        {
            get
            {
                return _attackMission.Id;
            }
        }

        public TimeSpan ArrivalTimeUtc
        {
            get
            {
                return _attackMission.ArrivalTimeUtc;
            }
        }

        public MissionPlanet PlanetFrom
        {
            get
            {
                return _attackMission.PlanetFrom;
            }
        }

        public MissionPlanet PlanetTo
        {
            get
            {
                return _attackMission.PlanetTo;
            }
        }

        public override int GetHashCode()
        {
            return _attackMission.Id.GetHashCode();
        }
    }
}