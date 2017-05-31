using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Domain.Services.Interfaces;

namespace OGame.Bot.Domain.Services.Implementations
{
    public class UserPlanetsService : IUserPlanetsService
    {
        public Task<IEnumerable<UserPlanet>> GetAllUserPlanetsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsItUserPlanetAsync(MissionPlanet planet)
        {
            throw new NotImplementedException();
        }
    }
}