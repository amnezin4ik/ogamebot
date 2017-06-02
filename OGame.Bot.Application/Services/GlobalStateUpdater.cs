using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain;
using OGame.Bot.Domain.Services.Interfaces;

namespace OGame.Bot.Application.Services
{
    public class GlobalStateUpdater : IGlobalStateUpdater
    {
        private readonly Logger _logger = LogManager.GetLogger(nameof(GlobalStateUpdater));
        private readonly IMessageServiceBus _messageServiceBus;
        private readonly IMissionService _missionService;
        private Task _runTask;
        private CancellationTokenSource _runCancellationTokenSource;

        public GlobalStateUpdater(IMessageServiceBus messageServiceBus, IMissionService missionService)
        {
            _messageServiceBus = messageServiceBus;
            _missionService = missionService;
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

        private async Task<IEnumerable<Message>> GetNewMessagesAsync()
        {
            var newMessages = new List<Message>();

            var attackMessages = await GetAttackMessagesAsync();
            newMessages.AddRange(attackMessages);

            return newMessages;
        }

        private async Task<IEnumerable<Message>> GetAttackMessagesAsync()
        {
            var attackMissions = await _missionService.GetMissionsAsync(MissionType.Attak);
            var attackMessages = attackMissions.Select(am => new AttackMessage(am)).ToList();
            return attackMessages;
        }
    }
}