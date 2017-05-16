using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IAuthorizationClient
    {
        Task<SessionData> LogInAsync(string userName = "user7395496", string password = "2016895");
    }
}