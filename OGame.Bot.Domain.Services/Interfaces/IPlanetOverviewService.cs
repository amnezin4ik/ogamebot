using System.Threading.Tasks;

namespace OGame.Bot.Domain.Services.Interfaces
{
    public interface IPlanetOverviewService
    {
        Task<PlanetOverview> GetPlanetOverviewAsync(UserPlanet userPlanet);
    }
}