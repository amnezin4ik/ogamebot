using Caliburn.Micro;
using OGame.Bot.Wpf.Models;

namespace OGame.Bot.Wpf.ViewModels
{
    public class BotControlPanelViewModel : Screen
    {
        public BotControlPanelViewModel()
        {
            CurrentUser = new User
            {
                UserName = "user7395496",
                Password = "2016895"
            };
        }

        private User _currentUser;

        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                NotifyOfPropertyChange(nameof(CurrentUser));
            }
        }

        public async void RunBot()
        {

        }
    }
}