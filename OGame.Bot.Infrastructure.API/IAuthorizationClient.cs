using System.Net;
using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Models;

namespace OGame.Bot.Infrastructure.API
{
    public interface IAuthorizationClient
    {
        Task<SessionData> LogInAsync(string userName = "user7395496", string password = "2016895");
    }
}