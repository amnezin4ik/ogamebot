using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.Services
{
    public class GlobalStateUpdater : IGlobalStateUpdater
    {
        private readonly Logger _logger = LogManager.GetLogger(nameof(GlobalStateUpdater));
        private readonly IMessageServiceBus _messageServiceBus;
        private Task _runTask;
        private CancellationTokenSource _runCancellationTokenSource;

        public GlobalStateUpdater(IMessageServiceBus messageServiceBus)
        {
            _messageServiceBus = messageServiceBus;
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }

        public void Run()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Run method was already called");
            }
            IsRunning = true;

            _runCancellationTokenSource = new CancellationTokenSource();
            _runTask = Task.Run(async () =>
            {
                while (!_runCancellationTokenSource.IsCancellationRequested)
                {
                    var messages = await GetNewMessagesAsync();
                    foreach (var message in messages)
                    {
                        _messageServiceBus.AddMessage(message);
                    }
                    var delayInMinutes = new Random().Next(5, 10);
                    await Task.Delay(TimeSpan.FromMinutes(delayInMinutes), _runCancellationTokenSource.Token);
                }
            }, _runCancellationTokenSource.Token);
        }

        public async Task StopAsync()
        {
            if (IsRunning)
            {
                _runCancellationTokenSource.Cancel();
                try
                {
                    await _runTask;
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    throw;
                }
                finally
                {
                    _runCancellationTokenSource.Dispose();
                }
            }
            IsRunning = false;
        }

        private async Task<IEnumerable<Message>> GetNewMessagesAsync()
        {
            //TODO: check current game state, create and return necessary messages. maybe it should be separate service
            throw new NotImplementedException();
        }
    }
}