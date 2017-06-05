using System.Threading.Tasks;

namespace OGame.Bot.Domain.Services.Interfaces
{
    public interface IUpdatableSessionDataProvider : ISessionDataProvider
    {
        bool IsInitialized { get; }

        Task InitializeAsync(UserCredentials credentials);

        Task RefreshSessionDataAsync();
    }
}