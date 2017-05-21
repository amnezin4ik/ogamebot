using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IMissionClient
    {
        Task<IEnumerable<Mission>> GetAllMissionsAsync(SessionData sessionData);

        Task<IEnumerable<Mission>> GetMissionsAsync(SessionData sessionData, MissionType missionType);
    }
}