using System;
using System.Threading.Tasks;
using OGame.Bot.Domain.Services.Interfaces;

namespace OGame.Bot.Domain.Services.Implementations
{
    public class UpdatableSessionDataProvider : IUpdatableSessionDataProvider
    {
        public SessionData GetSessionData()
        {
            throw new NotImplementedException();
        }

        public Task UpdateSessionDataAsync()
        {
            throw new NotImplementedException();
        }
    }
}