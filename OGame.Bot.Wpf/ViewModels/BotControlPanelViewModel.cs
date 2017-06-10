using System;
using System.ComponentModel;
using AutoMapper;
using Caliburn.Micro;
using NLog;
using OGame.Bot.Application.Services;
using OGame.Bot.Modules.Common;
using OGame.Bot.Wpf.Models;

namespace OGame.Bot.Wpf.ViewModels
{
    public class BotControlPanelViewModel : Screen
    {
        private readonly IBotService _bot;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMapper _mapper;
        private readonly ILogger _logger = NLog.LogManager.GetLogger("BotControlPanelViewModel");
        private UserCredentials _currentUserCredentials;

        public BotControlPanelViewModel(IBotService bot, IDateTimeProvider dateTimeProvider, IMapper mapper)
        {
            _bot = bot;
            _dateTimeProvider = dateTimeProvider;
            _mapper = mapper;
            CurrentUserCredentials = new UserCredentials
            {
                UserName = "user7395496",
                Password = "2016895"
            };
            var memoryEventTarget = (MemoryEventTarget)NLog.LogManager.Configuration.FindTargetByName("memory");
            memoryEventTarget.EventReceived += MemoryEventTarget_EventReceived;

            for (int i = 0; i < 150; i++)
            {
                _logger.Info(i);
            }
        }

        public UserCredentials CurrentUserCredentials
        {
            get
            {
                return _currentUserCredentials;
            }
            set
            {
                _currentUserCredentials = value;
                NotifyOfPropertyChange(nameof(CurrentUserCredentials));
            }
        }

        public async void Run()
        {
            _logger.Info($"Run with username: {CurrentUserCredentials.UserName}, password: {CurrentUserCredentials.Password}, server: ");
            var credentials = _mapper.Map<UserCredentials, Application.Models.UserCredentials>(CurrentUserCredentials);
            await _bot.RunAsync(credentials);
        }

        public async void Stop()
        {
            _logger.Info("Stop");
            await _bot.StopAsync();
        }

        #region Logging

        private string _logOutput;
        public string LogOutput
        {
            get { return _logOutput; }
            set
            {
                _logOutput = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(LogOutput)));
            }
        }

        private void MemoryEventTarget_EventReceived(LogEventInfo obj)
        {
            var utcNow = _dateTimeProvider.GetUtcDateNow();
            LogOutput += $"{utcNow:HH:mm:ss} {obj.Level.ToString().ToUpper()} {obj.LoggerName} {obj.Message}{Environment.NewLine}";
        }
        #endregion
    }
}