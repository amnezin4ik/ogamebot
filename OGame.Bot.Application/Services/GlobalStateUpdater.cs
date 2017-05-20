using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.Services
{
    public class GlobalStateUpdater : IGlobalStateUpdater
    {
        private readonly int _minUpdateDelayInMinutes;
        private readonly int _maxUpdateDelayInMinutes;

        public GlobalStateUpdater(int minUpdateDelayInMinutes = 5, int maxUpdateDelayInMinutes = 10)
        {
            _minUpdateDelayInMinutes = minUpdateDelayInMinutes;
            _maxUpdateDelayInMinutes = maxUpdateDelayInMinutes;
        }

        public async Task RunAsync(IMessageServiceBus messageServiceBus, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var messages = await GetNewMessagesAsync();
                foreach (var message in messages)
                {
                    messageServiceBus.AddMessage(message);
                }
                var delayInMinutes = new Random().Next(_minUpdateDelayInMinutes, _maxUpdateDelayInMinutes);
                await Task.Delay(TimeSpan.FromMinutes(delayInMinutes), cancellationToken);
            }
        }
        
        private async Task<IEnumerable<Message>> GetNewMessagesAsync()
        {
            //TODO: check current game state, create and return necessary messages. maybe it should be separate service
            throw new NotImplementedException();
        }
    }
}