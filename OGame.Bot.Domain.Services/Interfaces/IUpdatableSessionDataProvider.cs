using System.Threading.Tasks;

namespace OGame.Bot.Domain.Services.Interfaces
{
    public interface IUpdatableSessionDataProvider : ISessionDataProvider
    {
        Task UpdateSessionDataAsync();
    }
}