using AutoMapper;
using Caliburn.Micro;
using OGame.Bot.Application.Services;
using OGame.Bot.Wpf.Models;

namespace OGame.Bot.Wpf.ViewModels
{
    public class BotControlPanelViewModel : Screen
    {
        private readonly IBotService _bot;
        private readonly IMapper _mapper;

        public BotControlPanelViewModel(IBotService bot, IMapper mapper)
        {
            _bot = bot;
            _mapper = mapper;
            CurrentUserCredentials = new UserCredentials
            {
                UserName = "user7395496",
                Password = "2016895"
            };
        }

        private UserCredentials _currentUserCredentials;

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
            var credentials = _mapper.Map<UserCredentials, Application.Models.UserCredentials>(CurrentUserCredentials);
            await _bot.RunAsync(credentials);
        }

        public async void Stop()
        {
            await _bot.StopAsync();
        }
    }
}