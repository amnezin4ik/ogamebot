using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Infrastructure.API.APIClients;
using Dto = OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Domain.Services.Implementations
{
    public class UserPlanetsService : IUserPlanetsService
    {
        private readonly IUserPlanetsClient _userPlanetsClient;
        private readonly ISessionDataProvider _sessionDataProvider;
        private readonly IMapper _mapper;

        public UserPlanetsService(IUserPlanetsClient userPlanetsClient, ISessionDataProvider sessionDataProvider, IMapper mapper)
        {
            _userPlanetsClient = userPlanetsClient;
            _sessionDataProvider = sessionDataProvider;
            _mapper = mapper;
        }

        //TODO: need to cache this data like SessionDataProvider and update it by GlobalStateUpdater
        public async Task<IEnumerable<UserPlanet>> GetAllUserPlanetsAsync()
        {
            var sessionData = _sessionDataProvider.GetSessionData();
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            var userPlanetsDtos = await _userPlanetsClient.GetUserPlanetsAsync(sessionDataDto);
            var userPlanets = _mapper.Map<IEnumerable<Dto.UserPlanet>, IEnumerable<UserPlanet>>(userPlanetsDtos);
            return userPlanets;
        }

        public async Task<bool> IsItUserPlanetAsync(MissionPlanet planet)
        {
            var userPlanets = await GetAllUserPlanetsAsync();
            var isItUserPlanet = userPlanets.Any(p => p.Coordinates == planet.Coordinates);
            return isItUserPlanet;
        }

        public async Task MakePlanetActiveAsync(string planetId)
        {
            var sessionData = _sessionDataProvider.GetSessionData();
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            await _userPlanetsClient.MakePlanetActiveAsync(sessionDataDto, planetId);
        }

        public async Task MakePlanetActiveAsync(Coordinates planetCoordinates)
        {
            var sessionData = _sessionDataProvider.GetSessionData();
            var sessionDataDto = _mapper.Map<SessionData, Dto.SessionData>(sessionData);
            var userPlanets = await GetAllUserPlanetsAsync();
            var planetToActivate = userPlanets.Single(p => p.Coordinates == planetCoordinates);
            await _userPlanetsClient.MakePlanetActiveAsync(sessionDataDto, planetToActivate.PlanetId);
        }
    }
}