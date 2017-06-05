using System.Threading.Tasks;
using AutoMapper;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Application.Models;
using OGame.Bot.Domain.Services.Interfaces;

namespace OGame.Bot.Application.Services
{
    public class BotService : IBotService
    {
        private readonly IMessageServiceBus _messageServiceBus;
        private readonly IGlobalStateUpdater _globalStateUpdater;
        private readonly IUpdatableSessionDataProvider _updatableSessionDataProvider;
        private readonly IMapper _mapper;

        public BotService(
            IMessageServiceBus messageServiceBus, 
            IGlobalStateUpdater globalStateUpdater, 
            IUpdatableSessionDataProvider updatableSessionDataProvider,
            IMapper mapper)
        {
            _messageServiceBus = messageServiceBus;
            _globalStateUpdater = globalStateUpdater;
            _updatableSessionDataProvider = updatableSessionDataProvider;
            _mapper = mapper;
        }

        public async Task RunAsync(UserCredentials credentials)
        {
            var credentialsModel = _mapper.Map<UserCredentials, Domain.UserCredentials>(credentials);
            await _updatableSessionDataProvider.InitializeAsync(credentialsModel);
            _messageServiceBus.Run();
            _globalStateUpdater.Run();
        }

        public async Task StopAsync()
        {
            await _messageServiceBus.StopAsync();
            await _globalStateUpdater.StopAsync();
        }
    }
}