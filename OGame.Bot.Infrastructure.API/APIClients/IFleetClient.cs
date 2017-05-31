using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IFleetClient
    {
        Task<Fleet> GetFleetAvailableAsync(SessionData sessionData);

        void Go(MissionType missionType, bool takeAllResources = false);

        void MoveToAttentionPhase(Fleet fleet);

        void MoveToGoPhase(Coordinates missionCoordinates, MissionTarget target, FleetSpeed speed);
    }
}