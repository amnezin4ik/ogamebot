using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IFleetClient
    {
        Task<Fleet> GetFleetAvailableAsync(SessionData sessionData);

        Task SendFleetPhase1(SessionData sessionData, Fleet fleet, Coordinates coordinatesFrom);

        Task<SendFleetPhase3Info> SendFleetPhase2(SessionData sessionData, Fleet fleet, Coordinates coordinatesTo, MissionTarget target, MissionType missionType, FleetSpeed fleetSpeed);

        Task SendFleetPhase3(SessionData sessionData, SendFleetPhase3Info sendFleetPhase3Info, int metal, int crystal, int deuterium);
    }
}