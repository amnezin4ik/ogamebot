using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Domain.Messages;

namespace OGame.Bot.Application.Services
{
    public class GlobalStateUpdater : IGlobalStateUpdater
    {
        private readonly ILogger _logger = LogManager.GetLogger(nameof(GlobalStateUpdater));
        private readonly IMessageServiceBus _messageServiceBus;
        private Task _runTask;
        private CancellationTokenSource _runCancellationTokenSource;

        public GlobalStateUpdater(IMessageServiceBus messageServiceBus)
        {
            _messageServiceBus = messageServiceBus;
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }

        //TODO: It should generate messages for update only (without any API calls), but new message processors should provide necessary messages
        public void Run()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("RunAsync method was already called");
            }
            IsRunning = true;

            _runCancellationTokenSource = new CancellationTokenSource();
            _runTask = Task.Run(async () =>
            {
                while (!_runCancellationTokenSource.IsCancellationRequested)
                {
                    var allMessageTypes = Enum.GetValues(typeof(MessageType)).Cast<MessageType>().ToList();
                    var updateMessage = new UpdateStateMessage(allMessageTypes);
                    _messageServiceBus.AddMessage(updateMessage);
                    
                    var delay = GetRandomDelay();
                    await Task.Delay(delay, _runCancellationTokenSource.Token);
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

        private TimeSpan GetRandomDelay()
        {
            const int minDelaySeconds = 5 * 60;
            const int maxDelaySeconds = 7 * 60;
            var delayInSeconds = new Random().Next(minDelaySeconds, maxDelaySeconds);
            var delay = TimeSpan.FromSeconds(delayInSeconds);
            return delay;
        }
    }
}