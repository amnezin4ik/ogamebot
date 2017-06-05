using System.Threading.Tasks;
using OGame.Bot.Application.Models;

namespace OGame.Bot.Application.Services
{
    public interface IBotService
    {
        Task RunAsync(UserCredentials credentials);

        Task StopAsync();
    }
}