using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IFleetClient
    {
        Task<Fleet> GetFleetAvailableAsync(SessionData sessionData);

        Task MoveToAttentionPhase(SessionData sessionData, Fleet fleet, Coordinates coordinatesFrom);

        Task<GoPhaseInfo> MoveToGoPhase(SessionData sessionData, Fleet fleet, Coordinates coordinatesTo, MissionTarget target, MissionType missionType, FleetSpeed fleetSpeed);

        Task Go(SessionData sessionData, GoPhaseInfo goPhaseInfo, double metal, double crystal, double deuterium);
    }
}