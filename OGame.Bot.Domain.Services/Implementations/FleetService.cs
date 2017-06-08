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
        private readonly IMissionClient _missionClient;
        private readonly ISessionDataProvider _sessionDataProvider;
        private readonly IMapper _mapper;

        public FleetService(IFleetClient fleetClient, IMissionClient missionClient, ISessionDataProvider sessionDataProvider, IMapper mapper)
        {
            _fleetClient = fleetClient;
            _missionClient = missionClient;
            _sessionDataProvider = sessionDataProvider;
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

        public async Task<Mission> SendFleetAsync(Fleet fleet, Coordinates coordinatesFrom, Coordinates coordinatesTo, MissionTarget missionTarget, MissionType missionType, FleetSpeed fleetSpeed, Resources resources)
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

            var oldMissionsDtos = await _missionClient.GetMissionsAsync(sessionDataDto, missionTypeDto);
            var oldMissionsIds = oldMissionsDtos.Select(m => m.Id).ToList();

            await SendFleet(sessionDataDto, fleetDto, coordinatesFromDto, coordinatesToDto, missionTargetDto, missionTypeDto, fleetSpeedDto, resourcesDto);

            var currentMissionsDtos = await _missionClient.GetMissionsAsync(sessionDataDto, missionTypeDto);
            var currentMissions = _mapper.Map<IEnumerable<Dto.Mission>, IEnumerable<Mission>>(currentMissionsDtos);
            var currentlyCreatedMission = GetCurrentlyCreatedMission(currentMissions, coordinatesTo, oldMissionsIds);
            return currentlyCreatedMission;
        }

        private Mission GetCurrentlyCreatedMission(IEnumerable<Mission> currentMissions, Coordinates coordinatesTo, List<string> existingMissionsIds)
        {
            throw new NotImplementedException();
            //var newMissions = currentMissions
            //    .Where(m => m.IsReturn == false &&
            //                m.PlanetTo.Coordinates == coordinatesTo &&
            //                !existingMissionsIds.Contains(m.Id))
            //    .ToList();
            //if (newMissions.Count != 1)
            //{
            //    var errorMessageBuilder = new StringBuilder("Can't recognize save mission, available missions:");
            //    foreach (var mission in newMissions)
            //    {
            //        errorMessageBuilder.AppendLine($"{mission.Id} ({mission.MissionType}): from {mission.PlanetFrom.Coordinates} to {mission.PlanetTo.Coordinates}. Arrival Time {mission.ArrivalTimeUtc}");
            //    }
            //    throw new AmbiguousMatchException(errorMessageBuilder.ToString());
            //}
            //var saveMission = newMissions.Single();
            //return saveMission;
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