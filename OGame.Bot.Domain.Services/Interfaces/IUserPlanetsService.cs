using System.Collections.Generic;
using System.Threading.Tasks;

namespace OGame.Bot.Domain.Services.Interfaces
{
    public interface IUserPlanetsService
    {
        Task<IEnumerable<UserPlanet>> GetAllUserPlanetsAsync();

        Task<bool> IsItUserPlanetAsync(MissionPlanet planet);
    }
}
