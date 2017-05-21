using System.Threading.Tasks;

namespace OGame.Bot.Domain.Services.Interfaces
{
    public interface IGalaxyService
    {
        Task<MissionPlanet> GetNearestInactivePlanetAsync(int minimumDistanceGalaxies);
    }
}