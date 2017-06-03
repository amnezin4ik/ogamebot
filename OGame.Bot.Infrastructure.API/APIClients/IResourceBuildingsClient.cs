using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IResourceBuildingsClient
    {
        Task GetResourceBuildingsAsync(SessionData sessionData, ResourceBuildingType resourceBuildingType);
    }
}