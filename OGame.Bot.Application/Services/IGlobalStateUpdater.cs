using System.Threading;

namespace OGame.Bot.Application.Services
{
    public interface IGlobalStateUpdater
    {
        void Run(CancellationToken cancellationToken);
    }
}