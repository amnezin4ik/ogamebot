using System.Threading.Tasks;
using AutoMapper;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Infrastructure.API.APIClients;
using Dto = OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Domain.Services.Implementations
{
    public class PlanetOverviewService : IPlanetOverviewService
    {
        private readonly IPlanetOverviewClient _planetOverviewClient;
        private readonly ISessionDataProvider _sessionDataProvider;
        private readonly IMapper _mapper;

        public PlanetOverviewService(IPlanetOverviewClient planetOverviewClient, ISessionDataProvider sessionDataProvider, IMapper mapper)
        {
            _planetOverviewClient = planetOverviewClient;
            _sessionDataProvider = sessionDataProvider;
            _mapper = mapper;
        }

        public async Task<PlanetOverview> GetPlanetOverviewAsync(UserPlanet userPlanet)
        {
            var sessionData = _sessionDataProvider.GetSessionData();
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            var userPlanetDto = _mapper.Map<UserPlanet, Dto.UserPlanet>(userPlanet);
            var fleetAvailableDto = await _planetOverviewClient.GetPlanetOverviewAsync(sessionDataDto, userPlanetDto);
            var fleetAvailable = _mapper.Map<Dto.PlanetOverview, PlanetOverview>(fleetAvailableDto);
            return fleetAvailable;
        }
    }
}