using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IResourcesClient
    {
        Task<ResourcesOverview> GetResourcesOverviewAsync(SessionData requestData);
    }
}