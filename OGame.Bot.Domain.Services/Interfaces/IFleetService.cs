using System.Threading.Tasks;

namespace OGame.Bot.Domain.Services.Interfaces
{
    public interface IFleetService
    {
        Task<Mission> SaveFleetAndResourcesAsync(MissionPlanet needSavePlanet, MissionPlanet destinationPlanet, FleetSpeed speed);
    }
}
