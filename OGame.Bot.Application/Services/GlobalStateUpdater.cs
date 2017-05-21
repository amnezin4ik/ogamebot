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
        public async Task Run(IMessageServiceBus messageServiceBus, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var messages = await GetNewMessagesAsync();
                foreach (var message in messages)
                {
                    messageServiceBus.AddMessage(message);
                }
                var delayInMinutes = new Random().Next(5, 10);
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