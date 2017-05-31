using System;
using System.Threading.Tasks;
using OGame.Bot.Domain.Services.Interfaces;

namespace OGame.Bot.Domain.Services.Implementations
{
    public class GalaxyService : IGalaxyService
    {
        public Task<MissionPlanet> GetNearestInactivePlanetAsync(int minimumDistanceGalaxies = 0)
        {
            throw new NotImplementedException();
        }
    }
}