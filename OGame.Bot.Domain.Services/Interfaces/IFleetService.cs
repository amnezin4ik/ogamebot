using System.Threading.Tasks;

namespace OGame.Bot.Domain.Services.Interfaces
{
    public interface IFleetService
    {
        Task<Fleet> GetActivePlanetFleetAsync();

        int CalculateFleetCapacity(Fleet fleet);

        Task<FleetMovement> SendFleetAsync(Fleet fleet, Coordinates coordinatesFrom, Coordinates coordinatesTo, MissionTarget missionTarget, MissionType missionType, FleetSpeed fleetSpeed, Resources resources);
    }
}