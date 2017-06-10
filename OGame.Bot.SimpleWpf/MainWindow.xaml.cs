using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using AutoMapper;
using NLog;
using OGame.Bot.Application.Services;
using OGame.Bot.Modules.Common;
using OGame.Bot.SimpleWpf.Annotations;
using UserCredentials = OGame.Bot.SimpleWpf.Models.UserCredentials;

namespace OGame.Bot.SimpleWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IBotService _bot;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMapper _mapper;
        private readonly ILogger _logger = LogManager.GetLogger("BotControlPanelViewModel");
        private UserCredentials _currentUserCredentials;

        public MainWindow()
        {
            InitializeComponent();

            var subjectProvider = new SubjectProvider();
            _bot = subjectProvider.Create<IBotService>();
            _dateTimeProvider = subjectProvider.Create<IDateTimeProvider>();
            _mapper = subjectProvider.Create<IMapper>();

            CurrentUserCredentials = new UserCredentials
            {
                UserName = "user7395496",
                Password = "2016895"
            };

            DataContext = CurrentUserCredentials;

            var memoryEventTarget = (MemoryEventTarget)LogManager.Configuration.FindTargetByName("memory");
            memoryEventTarget.EventReceived += MemoryEventTarget_EventReceived;
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
                OnPropertyChanged(nameof(CurrentUserCredentials));
            }
        }

        #region Logging

        private void MemoryEventTarget_EventReceived(LogEventInfo obj)
        {
            Dispatcher.Invoke(() =>
            {
                var utcNow = _dateTimeProvider.GetUtcDateNow();
                var logString = $"{utcNow:HH:mm:ss} {obj.Level.ToString().ToUpper()} {obj.LoggerName} {obj.FormattedMessage}{Environment.NewLine}";
                TextBoxLogOutput.AppendText(logString);
                TextBoxLogOutput.CaretIndex = TextBoxLogOutput.Text.Length;
                TextBoxLogOutput.ScrollToEnd();
            });
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Run_Click(object sender, RoutedEventArgs e)
        {
            _logger.Info($"Run with username: {CurrentUserCredentials.UserName}, password: {CurrentUserCredentials.Password}, server: ");
            var credentials = _mapper.Map<UserCredentials, Application.Models.UserCredentials>(CurrentUserCredentials);
            await _bot.RunAsync(credentials);
        }

        private async void Stop_Click(object sender, RoutedEventArgs e)
        {
            _logger.Info("Stop");
            await _bot.StopAsync();
        }
    }
}