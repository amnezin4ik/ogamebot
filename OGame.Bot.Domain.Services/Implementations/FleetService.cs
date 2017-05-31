using System;
using System.Threading.Tasks;
using OGame.Bot.Domain.Services.Interfaces;

namespace OGame.Bot.Domain.Services.Implementations
{
    public class FleetService : IFleetService
    {
        public Task<Mission> SaveFleetAndResourcesAsync(MissionPlanet needSavePlanet, MissionPlanet destinationPlanet, FleetSpeed speed)
        {
            throw new NotImplementedException();
        }
    }
}