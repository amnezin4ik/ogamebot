using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.Application.MessageProcessors
{
    public class AttackMessageProcessor : IAttackMessageProcessor
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IFleetService _fleetService;
        private readonly IGalaxyService _galaxyService;
        private readonly IUserPlanetsService _userPlanetsService;
        private readonly IMapper _mapper;

        public AttackMessageProcessor(
            IDateTimeProvider dateTimeProvider, 
            IFleetService fleetService, 
            IGalaxyService galaxyService,
            IUserPlanetsService userPlanetsService,
            IMapper mapper)
        {
            _dateTimeProvider = dateTimeProvider;
            _fleetService = fleetService;
            _galaxyService = galaxyService;
            _userPlanetsService = userPlanetsService;
            _mapper = mapper;
        }

        public bool CanProcess(Message message)
        {
            return message.MessageType == MessageType.Attack;
        }

        public bool ShouldProcessRightNow(Message message)
        {
            var attackMessage = message as AttackMessage;
            if (attackMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            var timeToAttack = attackMessage.ArrivalTimeUtc - _dateTimeProvider.GetUtcNow();
            var shouldProcess = timeToAttack.TotalSeconds < 100;
            return shouldProcess;
        }

        public async Task<IEnumerable<Message>> ProcessAsync(Message message)
        {
            var attackMessage = message as AttackMessage;
            if (attackMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            var resultMessages = new List<Message>();
            var weAttaked = await _userPlanetsService.IsItUserPlanetAsync(attackMessage.PlanetTo);
            if (weAttaked)
            {
                MissionPlanet destinationPlanet;
                var userPlanets = await _userPlanetsService.GetAllUserPlanetsAsync();
                if (userPlanets.Count() > 1)
                {
                    var anotherUserPlanet = userPlanets.First(p => p.Coordinates != attackMessage.PlanetTo.Coordinates);
                    destinationPlanet = _mapper.Map<UserPlanet, MissionPlanet>(anotherUserPlanet);
                }
                else
                {
                    destinationPlanet = await _galaxyService.GetNearestInactivePlanetAsync();
                }
                var saveMission = await SaveFleetAndResourcesAsync(attackMessage.PlanetTo, destinationPlanet);
                var approximateStartOfReturn = GetApproximateStartOfReturn(attackMessage);
                var returnFleetMessage = new ReturnFleetMessage(saveMission, approximateStartOfReturn);
                resultMessages.Add(returnFleetMessage);
            }
            return resultMessages;
        }

        private async Task<Mission> SaveFleetAndResourcesAsync(MissionPlanet needSavePlanet, MissionPlanet destinationPlanet)
        {
            //await _userPlanetsService.MakePlanetActiveAsync(needSavePlanet.Coordinates);
            throw new NotImplementedException();
        }

        private TimeSpan GetApproximateStartOfReturn(AttackMessage attackMessage)
        {
            var utcNow = _dateTimeProvider.GetUtcNow();
            var timeToAttack = attackMessage.ArrivalTimeUtc - utcNow;
            var halfTimeToAttack = timeToAttack.Ticks / 2;
            var approximateStartOfReturn = utcNow + TimeSpan.FromTicks(halfTimeToAttack) + TimeSpan.FromSeconds(5);
            return approximateStartOfReturn;
        }
    }
}