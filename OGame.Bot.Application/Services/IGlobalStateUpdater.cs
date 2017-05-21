using System.Threading;
using System.Threading.Tasks;

namespace OGame.Bot.Application.Services
{
    public interface IGlobalStateUpdater
    {
        Task Run(CancellationToken cancellationToken);
    }
}