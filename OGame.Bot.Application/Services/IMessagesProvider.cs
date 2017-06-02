using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.Services
{
    public interface IMessagesProvider
    {
        Task<IEnumerable<Message>> GetNewMessagesAsync();
    }
}