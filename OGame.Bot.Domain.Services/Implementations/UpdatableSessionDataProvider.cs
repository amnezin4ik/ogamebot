using System;
using System.Threading.Tasks;
using AutoMapper;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Infrastructure.API.APIClients;

namespace OGame.Bot.Domain.Services.Implementations
{
    public class UpdatableSessionDataProvider : IUpdatableSessionDataProvider
    {
        private readonly IAuthorizationClient _authorizationClient;
        private readonly IMapper _mapper;
        private UserCredentials _credentials;
        private SessionData _sessionData;

        public UpdatableSessionDataProvider(IAuthorizationClient authorizationClient, IMapper mapper)
        {
            _authorizationClient = authorizationClient;
            _mapper = mapper;
            IsInitialized = false;
        }

        public bool IsInitialized { get; private set; }

        public async Task InitializeAsync(UserCredentials credentials)
        {
            _credentials = credentials;
            var sessionDataDto = await _authorizationClient.LogInAsync(_credentials.UserName, _credentials.Password);
            _sessionData = _mapper.Map<Infrastructure.API.Dto.SessionData, SessionData>(sessionDataDto);
            IsInitialized = true;
        }

        public SessionData GetSessionData()
        {
            EnsureIsInitialized();
            return _sessionData;
        }

        public async Task RefreshSessionDataAsync()
        {
            EnsureIsInitialized();
            var sessionDataDto = await _authorizationClient.LogInAsync(_credentials.UserName, _credentials.Password);
            _sessionData = _mapper.Map<Infrastructure.API.Dto.SessionData, SessionData>(sessionDataDto);
        }

        private void EnsureIsInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException($"Call {nameof(InitializeAsync)} method before use");
            }
        }
    }
}