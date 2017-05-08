using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Models;

namespace OGame.Bot.Infrastructure.API
{
    public interface IResourcesClient
    {
        Task<ResourcesOverview> GetResourcesOverviewAsync(SessionData requestData);
    }
}