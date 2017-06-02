using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Application.MessageBus;

namespace OGame.Bot.Application.Services
{
    public class GlobalStateUpdater : IGlobalStateUpdater
    {
        private readonly Logger _logger = LogManager.GetLogger(nameof(GlobalStateUpdater));
        private readonly IMessageServiceBus _messageServiceBus;
        private readonly IMessagesProvider _messagesProvider;
        private Task _runTask;
        private CancellationTokenSource _runCancellationTokenSource;

        public GlobalStateUpdater(IMessageServiceBus messageServiceBus, IMessagesProvider messagesProvider)
        {
            _messageServiceBus = messageServiceBus;
            _messagesProvider = messagesProvider;
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
                    var messages = await _messagesProvider.GetNewMessagesAsync();
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
                    if (_runTask.Status != TaskStatus.Canceled && 
                        _runTask.Status != TaskStatus.WaitingForActivation)
                    {
                        await _runTask;
                    }
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
    }
}