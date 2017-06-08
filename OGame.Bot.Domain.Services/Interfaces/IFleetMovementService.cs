using System.Collections.Generic;
using System.Threading.Tasks;

namespace OGame.Bot.Domain.Services.Interfaces
{
    public interface IFleetMovementService
    {
        Task<IEnumerable<FleetMovement>> GetAllFleetMovementsAsync();
        Task<IEnumerable<FleetMovement>> GetFleetMovementsAsync(MissionType missionType);
        Task<bool> IsFleetMovementStillExistsAsync(string fleetMovementId);
        Task ReturnFleetAsync(string fleetMovementId);
    }
}