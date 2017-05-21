using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.Application.MessageProcessors
{
    public class AttackMessageProcessor : IAttackMessageProcessor
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IFleetService _fleetService;
        private readonly IGalaxyService _galaxyService;
        private readonly IUserPlanetsService _userPlanetsService;
        private readonly IMapper _mapper;

        public AttackMessageProcessor(
            IDateTimeProvider dateTimeProvider, 
            IFleetService fleetService, 
            IGalaxyService galaxyService,
            IUserPlanetsService userPlanetsService,
            IMapper mapper)
        {
            _dateTimeProvider = dateTimeProvider;
            _fleetService = fleetService;
            _galaxyService = galaxyService;
            _userPlanetsService = userPlanetsService;
            _mapper = mapper;
        }

        public bool CanProcess(Message message)
        {
            return message.MessageType == MessageType.Attack;
        }

        public async Task<IEnumerable<Message>> ProcessAsync(Message message)
        {
            var attackMessage = message as AttackMessage;
            if (attackMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            var resultMessages = new List<Message>();
            var weAttaked = await _userPlanetsService.IsItUserPlanetAsync(attackMessage.PlanetTo);
            if (weAttaked)
            {
                var userPlanets = await _userPlanetsService.GetAllUserPlanetsAsync();
                MissionPlanet planetToSave;
                if (userPlanets.Count() > 1)
                {
                    var anotherUserPlanet = userPlanets.First(p => p.Coordinates != attackMessage.PlanetTo.Coordinates);
                    planetToSave = _mapper.Map<UserPlanet, MissionPlanet>(anotherUserPlanet);
                }
                else
                {
                    planetToSave = await _galaxyService.GetNearestInactivePlanetAsync(40);
                }
                var saveMission = await _fleetService.SaveFleetAndResourcesAsync(attackMessage.PlanetTo, planetToSave, FleetSpeed.Percent10);
                var returnFleetMessage = new ReturnFleetMessage(saveMission);
                resultMessages.Add(returnFleetMessage);
            }
            return resultMessages;
        }

        public bool ShouldProcessRightNow(Message message)
        {
            var attackMessage = message as AttackMessage;
            if (attackMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            var timeToAttack = attackMessage.ArrivalTimeUtc - _dateTimeProvider.GetUtcNow();
            var shouldProcess = timeToAttack.TotalSeconds < 100;
            return shouldProcess;
        }
    }
}