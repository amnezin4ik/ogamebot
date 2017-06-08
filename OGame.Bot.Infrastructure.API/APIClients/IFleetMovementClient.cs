using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IFleetMovementClient
    {
        Task<IEnumerable<FleetMovement>> GetAllFleetMovementsAsync(SessionData sessionData);

        Task<IEnumerable<FleetMovement>> GetFleetMovementsAsync(SessionData sessionData, MissionType missionType);

        Task ReturnFleetAsync(SessionData sessionData, string fleetMovementId);
    }
}