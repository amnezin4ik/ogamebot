using Caliburn.Micro;

namespace OGame.Bot.Wpf.ViewModels
{
    public class MainWindowViewModel : Conductor<IScreen>
    {
        private readonly Services.INavigationService _navigationService;

        public MainWindowViewModel(Services.INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        protected override void OnInitialize()
        {
            //_navigationService.NavigateToViewModel(typeof(BotControlPanelViewModel));
            _navigationService.Initialize(this);
            _navigationService.Navigate<BotControlPanelViewModel>();
        }
    }
}