using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OGame.Bot.Application.MessageProcessors.Interfaces;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.Application.MessageProcessors.Implementations
{
    public class AttackMessageProcessor : IAttackMessageProcessor
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IFleetService _fleetService;
        private readonly IGalaxyService _galaxyService;
        private readonly IUserPlanetsService _userPlanetsService;
        private readonly IMissionService _missionService;
        private readonly IPlanetOverviewService _planetOverviewService;
        private readonly IMapper _mapper;

        public AttackMessageProcessor(
            IDateTimeProvider dateTimeProvider, 
            IFleetService fleetService, 
            IGalaxyService galaxyService,
            IUserPlanetsService userPlanetsService,
            IMissionService missionService,
            IPlanetOverviewService planetOverviewService,
            IMapper mapper)
        {
            _dateTimeProvider = dateTimeProvider;
            _fleetService = fleetService;
            _galaxyService = galaxyService;
            _userPlanetsService = userPlanetsService;
            _missionService = missionService;
            _planetOverviewService = planetOverviewService;
            _mapper = mapper;
        }

        public bool CanProcess(Message message)
        {
            return message.MessageType == MessageType.Attack;
        }

        public bool ShouldProcessRightNow(Message message)
        {
            var attackMessage = message as AttackMessage;
            if (attackMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            var timeToAttack = attackMessage.ArrivalTimeUtc - _dateTimeProvider.GetUtcNow();
            var shouldProcess = timeToAttack.TotalSeconds < 60;
            return shouldProcess;
        }

        public async Task<IEnumerable<Message>> ProcessAsync(Message message)
        {
            var attackMessage = message as AttackMessage;
            if (attackMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            var resultMessages = new List<Message>();
            var weAttaked = await _userPlanetsService.IsItUserPlanetAsync(attackMessage.PlanetTo.Coordinates);
            if (weAttaked)
            {
                var isMissionStillExists = await _missionService.IsFleetMovementStillExistsAsync(attackMessage.MissionId);
                if (isMissionStillExists)
                {
                    MissionPlanet destinationPlanet;
                    var userPlanets = await _userPlanetsService.GetAllUserPlanetsAsync();
                    if (userPlanets.Count() > 1)
                    {
                        var anotherUserPlanet = userPlanets.First(p => p.Coordinates != attackMessage.PlanetTo.Coordinates);
                        destinationPlanet = _mapper.Map<UserPlanet, MissionPlanet>(anotherUserPlanet);
                    }
                    else
                    {
                        destinationPlanet = await _galaxyService.GetNearestInactivePlanetAsync();
                    }
                    var userPlanetToSave = await _userPlanetsService.GetUserPlanetAsync(attackMessage.PlanetTo.Coordinates);
                    var saveMovement = await SaveFleetAndResourcesAsync(userPlanetToSave, destinationPlanet);
                    if (saveMovement != null)
                    {
                        var approximateStartOfReturn = GetApproximateStartOfReturn(attackMessage);
                        var returnFleetMessage = new ReturnFleetMessage(saveMovement, approximateStartOfReturn);
                        resultMessages.Add(returnFleetMessage);
                    }
                }
            }
            return resultMessages;
        }

        private async Task<FleetMovement> SaveFleetAndResourcesAsync(UserPlanet needSavePlanet, MissionPlanet destinationPlanet)
        {
            FleetMovement saveFleetMovement = null;
            await _userPlanetsService.MakePlanetActiveAsync(needSavePlanet);
            var availableFleet = await _fleetService.GetActivePlanetFleetAsync();
            var hasAnyShip = availableFleet.ShipCells.Any(s => s.Count > 0);
            if (hasAnyShip)
            {
                var needSavePlanetOverview = await _planetOverviewService.GetPlanetOverviewAsync(needSavePlanet);
                saveFleetMovement = await _fleetService.SendFleetAsync(availableFleet, needSavePlanet.Coordinates, destinationPlanet.Coordinates, MissionTarget.Planet, MissionType.Leave, FleetSpeed.Percent10, needSavePlanetOverview.Resources);
            }
            return saveFleetMovement;
        }

        private TimeSpan GetApproximateStartOfReturn(AttackMessage attackMessage)
        {
            var utcNow = _dateTimeProvider.GetUtcNow();
            var timeToAttack = attackMessage.ArrivalTimeUtc - utcNow;
            var halfTimeToAttack = timeToAttack.Ticks / 2;
            var approximateStartOfReturn = utcNow + TimeSpan.FromTicks(halfTimeToAttack) + TimeSpan.FromSeconds(2);
            return approximateStartOfReturn;
        }
    }
}