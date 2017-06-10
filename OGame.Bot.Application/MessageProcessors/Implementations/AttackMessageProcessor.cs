using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NLog;
using OGame.Bot.Application.MessageProcessors.Interfaces;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.Application.MessageProcessors.Implementations
{
    public class AttackMessageProcessor : IAttackMessageProcessor
    {
        private readonly Logger _logger = LogManager.GetLogger(nameof(AttackMessageProcessor));
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

            _logger.Info($"Processing {attackMessage.MissionId} mission.");
            var resultMessages = new List<Message>();
            var weAttaked = await _userPlanetsService.IsItUserPlanetAsync(attackMessage.PlanetTo.Coordinates);
            if (weAttaked)
            {
                _logger.Info("We attacked.");
                var isMissionStillExists = await _missionService.IsFleetMovementStillExistsAsync(attackMessage.MissionId);
                if (isMissionStillExists)
                {
                    _logger.Info("attack mission still exists");
                    MissionPlanet destinationPlanet;
                    var userPlanets = await _userPlanetsService.GetAllUserPlanetsAsync();
                    if (userPlanets.Count() > 1)
                    {
                        var anotherUserPlanet = userPlanets.First(p => p.Coordinates != attackMessage.PlanetTo.Coordinates);
                        _logger.Info($"have another user planet - {anotherUserPlanet.Name} {anotherUserPlanet.Coordinates}");
                        destinationPlanet = _mapper.Map<UserPlanet, MissionPlanet>(anotherUserPlanet);
                    }
                    else
                    {
                        _logger.Info($"have mo another user planets, find nearest inactive planet");
                        destinationPlanet = await _galaxyService.GetNearestInactivePlanetAsync();
                        _logger.Info($"nearest inactive planet - {destinationPlanet.Name} {destinationPlanet.Coordinates}");
                    }

                    var userPlanetToSave = await _userPlanetsService.GetUserPlanetAsync(attackMessage.PlanetTo.Coordinates);
                    _logger.Info($"current user planet to save - {userPlanetToSave.Name} {userPlanetToSave.Coordinates}");

                    var saveMovement = await SaveFleetAndResourcesAsync(userPlanetToSave, destinationPlanet);
                    if (saveMovement != null)
                    {
                        _logger.Warn($"have save movement - {saveMovement}");
                        var approximateStartOfReturn = GetApproximateStartOfReturn(attackMessage);
                        _logger.Warn($"approximateStartOfReturn - {approximateStartOfReturn}");
                        var returnFleetMessage = new ReturnFleetMessage(saveMovement, approximateStartOfReturn);
                        resultMessages.Add(returnFleetMessage);
                    }
                    else
                    {
                        _logger.Warn("have NO save movement");
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
            _logger.Info($"availableFleet: {availableFleet}");
            var hasAnyShip = availableFleet.ShipCells.Any(s => s.Count > 0);
            if (hasAnyShip)
            {
                var needSavePlanetOverview = await _planetOverviewService.GetPlanetOverviewAsync(needSavePlanet);
                _logger.Info($"Save planet overview: {needSavePlanetOverview}");
                saveFleetMovement = await _fleetService.SendFleetAsync(availableFleet, needSavePlanet.Coordinates, destinationPlanet.Coordinates, MissionTarget.Planet, MissionType.Leave, FleetSpeed.Percent10, needSavePlanetOverview.Resources);
                _logger.Info("Fleet sended.");
                _logger.Info($"Fleet movement: {saveFleetMovement}");
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