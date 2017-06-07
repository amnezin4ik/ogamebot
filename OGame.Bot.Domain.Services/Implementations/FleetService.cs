using System;
using System.Linq;
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

            await _fleetClient.SendFleetPhase1(sessionDataDto, fleetDto, coordinatesFromDto);
            var moveToGoPhaseInfoDto = await _fleetClient.SendFleetPhase2(sessionDataDto, fleetDto, coordinatesToDto, missionTargetDto, missionTypeDto, fleetSpeedDto);
            await _fleetClient.SendFleetPhase3(sessionDataDto, moveToGoPhaseInfoDto, resourcesDto);

            var typeMissionsDtos = await _missionClient.GetMissionsAsync(sessionDataDto, missionTypeDto);

            var saveMissionDto = typeMissionsDtos.Where(m => m.PlanetTo.Coordinates == coordinatesToDto);
            //var saveMission = _mapper.Map<Dto.Mission, Mission>(saveMissionDto);
            //return saveMission;

            throw new NotImplementedException();
        }
    }
}