using System.Collections.Generic;
using System.Threading.Tasks;

namespace OGame.Bot.Domain.Services.Interfaces
{
    public interface IMissionService
    {
        Task<IEnumerable<Mission>> GetAllMissionsAsync();

        Task<IEnumerable<Mission>> GetMissionsAsync(MissionType missionType);
    }
}