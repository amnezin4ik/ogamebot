using System.Threading;
using System.Threading.Tasks;

namespace OGame.Bot.Application.Services
{
    public interface IGlobalStateUpdater
    {
        void Run();

        Task StopAsync();

        bool IsRunning { get; }
    }
}