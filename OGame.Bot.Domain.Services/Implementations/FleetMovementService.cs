using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Infrastructure.API.APIClients;
using Dto = OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Domain.Services.Implementations
{
    public class FleetMovementService : IFleetMovementService
    {
        private readonly IFleetMovementClient _fleetMovementClient;
        private readonly ISessionDataProvider _sessionDataProvider;
        private readonly IMapper _mapper;

        public FleetMovementService(IFleetMovementClient fleetMovementClient, ISessionDataProvider sessionDataProvider, IMapper mapper)
        {
            _fleetMovementClient = fleetMovementClient;
            _sessionDataProvider = sessionDataProvider;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FleetMovement>> GetAllFleetMovementsAsync()
        {
            var sessionData = _sessionDataProvider.GetSessionData();
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            var allFleetMovementsDtos = await _fleetMovementClient.GetAllFleetMovementsAsync(sessionDataDto);
            var allFleetMovements = _mapper.Map<IEnumerable<Dto.FleetMovement>, IEnumerable<FleetMovement>>(allFleetMovementsDtos);
            return allFleetMovements;
        }

        public async Task<IEnumerable<FleetMovement>> GetFleetMovementsAsync(MissionType missionType)
        {
            var sessionData = _sessionDataProvider.GetSessionData();
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            var missionTypeDto = _mapper.Map<MissionType, Dto.MissionType>(missionType);
            var missionsDtos = await _fleetMovementClient.GetFleetMovementsAsync(sessionDataDto, missionTypeDto);
            var missions = _mapper.Map<IEnumerable<Dto.FleetMovement>, IEnumerable<FleetMovement>>(missionsDtos);
            return missions;
        }

        public async Task<bool> IsFleetMovementStillExistsAsync(string fleetMovementId)
        {
            var sessionData = _sessionDataProvider.GetSessionData();
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            var allFleetMovementsDtos = await _fleetMovementClient.GetAllFleetMovementsAsync(sessionDataDto);
            var isFleetMovementStillExists = allFleetMovementsDtos.Any(m => m.Id == fleetMovementId);
            return isFleetMovementStillExists;
        }

        public async Task ReturnFleetAsync(string fleetMovementId)
        {
            var sessionData = _sessionDataProvider.GetSessionData();
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            await _fleetMovementClient.ReturnFleetAsync(sessionDataDto, fleetMovementId);
        }
    }
}