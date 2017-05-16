using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IFleetEventsClient
    {
        Task<IEnumerable<FleetEvent>> GetFleetEventsAsync(SessionData sessionData);
    }
}