using System.Collections.Generic;
using System.Threading.Tasks;

namespace OGame.Bot.Domain.Services.Interfaces
{
    public interface IMissionService
    {
        Task<IEnumerable<Mission>> GetAllMissionsAsync(SessionData sessionData);

        Task<IEnumerable<Mission>> GetMissionsAsync(SessionData sessionData, MissionType missionType);
    }
}