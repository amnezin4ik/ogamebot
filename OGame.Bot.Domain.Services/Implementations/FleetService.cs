using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Infrastructure.API.APIClients;
using Dto = OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Domain.Services.Implementations
{
    public class FleetService : IFleetService
    {
        private readonly IFleetClient _fleetClient;
        private readonly ISessionDataProvider _sessionDataProvider;
        private readonly IFleetMovementClient _fleetMovementClient;
        private readonly IMapper _mapper;

        public FleetService(IFleetClient fleetClient, ISessionDataProvider sessionDataProvider, IFleetMovementClient fleetMovementClient, IMapper mapper)
        {
            _fleetClient = fleetClient;
            _sessionDataProvider = sessionDataProvider;
            _fleetMovementClient = fleetMovementClient;
            _mapper = mapper;
        }

        public async Task<Fleet> GetActivePlanetFleetAsync()
        {
            var sessionData = _sessionDataProvider.GetSessionData();
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            var fleetAvailableDto = await _fleetClient.GetActivePlanetFleetAsync(sessionDataDto);
            var fleetAvailable = _mapper.Map<Dto.Fleet, Fleet>(fleetAvailableDto);
            return fleetAvailable;
        }

        public int CalculateFleetCapacity(Fleet fleet)
        {
            var capacity = 0;
            foreach (var shipCell in fleet.ShipCells)
            {
                var shipCapacity = shipCell.Ship.GetCapacity();
                var shipCellCapacity = shipCapacity * shipCell.Count;
                capacity += shipCellCapacity;
            }
            return capacity;
        }

        public async Task<FleetMovement> SendFleetAsync(Fleet fleet, Coordinates coordinatesFrom, Coordinates coordinatesTo, MissionTarget missionTarget, MissionType missionType, FleetSpeed fleetSpeed, Resources resources)
        {
            var sessionData = _sessionDataProvider.GetSessionData();
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            var fleetDto = _mapper.Map<Fleet, Dto.Fleet>(fleet);
            var coordinatesFromDto = _mapper.Map<Coordinates, Dto.Coordinates>(coordinatesFrom);
            var coordinatesToDto = _mapper.Map<Coordinates, Dto.Coordinates>(coordinatesTo);
            var missionTargetDto = _mapper.Map<MissionTarget, Dto.MissionTarget>(missionTarget);
            var missionTypeDto = _mapper.Map<MissionType, Dto.MissionType>(missionType);
            var fleetSpeedDto = _mapper.Map<FleetSpeed, Dto.FleetSpeed>(fleetSpeed);
            var resourcesDto = _mapper.Map<Resources, Dto.Resources>(resources);

            var oldFleetMovementsDtos = await _fleetMovementClient.GetFleetMovementsAsync(sessionDataDto, missionTypeDto);
            var oldFleetMovementsIds = oldFleetMovementsDtos.Select(m => m.Id).ToList();

            await SendFleet(sessionDataDto, fleetDto, coordinatesFromDto, coordinatesToDto, missionTargetDto, missionTypeDto, fleetSpeedDto, resourcesDto);

            var currentFleetMovementsDtos = await _fleetMovementClient.GetFleetMovementsAsync(sessionDataDto, missionTypeDto);
            var currentFleetMovements = _mapper.Map<IEnumerable<Dto.FleetMovement>, IEnumerable<FleetMovement>>(currentFleetMovementsDtos);
            var currentlyCreatedMovement = GetCurrentlyCreatedMission(currentFleetMovements, coordinatesTo, oldFleetMovementsIds);
            return currentlyCreatedMovement;
        }

        private FleetMovement GetCurrentlyCreatedMission(IEnumerable<FleetMovement> currentFleetMovements, Coordinates coordinatesTo, List<string> existingFleetMovementsIds)
        {
            var newFleetMovements = currentFleetMovements
                .Where(m => m.IsReturn == false &&
                            m.PlanetTo.Coordinates == coordinatesTo &&
                            !existingFleetMovementsIds.Contains(m.Id))
                .ToList();
            if (newFleetMovements.Count != 1)
            {
                var errorMessageBuilder = new StringBuilder("Can't recognize save fleet movement, available movements:");
                foreach (var mission in newFleetMovements)
                {
                    errorMessageBuilder.AppendLine($"{mission.Id} ({mission.MissionType}): from {mission.PlanetFrom.Coordinates} to {mission.PlanetTo.Coordinates}. Arrival Time {mission.ArrivalTimeUtc}");
                }
                throw new AmbiguousMatchException(errorMessageBuilder.ToString());
            }
            var saveMovement = newFleetMovements.Single();
            return saveMovement;
        }

        private async Task SendFleet(Dto.SessionData sessionDataDto, Dto.Fleet fleetDto, Dto.Coordinates coordinatesFromDto,
            Dto.Coordinates coordinatesToDto, Dto.MissionTarget missionTargetDto, Dto.MissionType missionTypeDto, Dto.FleetSpeed fleetSpeedDto,
            Dto.Resources resourcesDto)
        {
            await _fleetClient.SendFleetPhase1(sessionDataDto, fleetDto, coordinatesFromDto);
            var moveToGoPhaseInfoDto = await _fleetClient.SendFleetPhase2(sessionDataDto, fleetDto, coordinatesToDto, missionTargetDto, missionTypeDto, fleetSpeedDto);
            await _fleetClient.SendFleetPhase3(sessionDataDto, moveToGoPhaseInfoDto, resourcesDto);
        }
    }
}