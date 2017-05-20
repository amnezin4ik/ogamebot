using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Infrastructure.API.APIClients;
using Dto = OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Domain.Services.Implementations
{
    public class GlobalMissionService : IGlobalMissionService
    {
        private readonly IMissionClient _missionClient;
        private readonly IMapper _mapper;

        public GlobalMissionService(IMissionClient missionClient, IMapper mapper)
        {
            _missionClient = missionClient;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Mission>> GetAllMissionsAsync(SessionData sessionData)
        {
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            var allMissionsDtos = await _missionClient.GetAllMissionsAsync(sessionDataDto);
            var allMissions = _mapper.Map<IEnumerable<Dto.Mission>, IEnumerable<Mission>>(allMissionsDtos);
            return allMissions;
        }

        public async Task<IEnumerable<Mission>> GetMissionsAsync(SessionData sessionData, MissionType missionType)
        {
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            //TODO: use GetMissionsAsync API method
            var allMissionsDtos = await _missionClient.GetAllMissionsAsync(sessionDataDto);
            var allMissions = _mapper.Map<IEnumerable<Dto.Mission>, IEnumerable<Mission>>(allMissionsDtos);
            var targetMissions = allMissions.Where(m => m.MissionType == missionType);
            return targetMissions;
        }
    }
}
