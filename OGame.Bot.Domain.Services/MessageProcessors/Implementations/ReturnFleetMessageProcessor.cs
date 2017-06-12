using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Domain.Messages;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Domain.Services.MessageProcessors.Interfaces;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.Domain.Services.MessageProcessors.Implementations
{
    public class ReturnFleetMessageProcessor : IReturnFleetMessageProcessor
    {
        private readonly ILogger _logger = LogManager.GetLogger(nameof(ReturnFleetMessageProcessor));
        private readonly IFleetMovementService _fleetMovementService;
        private readonly IMissionService _missionService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReturnFleetMessageProcessor(IFleetMovementService fleetMovementService, IMissionService missionService, IDateTimeProvider dateTimeProvider)
        {
            _fleetMovementService = fleetMovementService;
            _missionService = missionService;
            _dateTimeProvider = dateTimeProvider;
        }

        public bool CanProcess(Message message)
        {
            return message.MessageType == MessageType.ReturnFleet;
        }

        public bool ShouldProcessRightNow(Message message)
        {
            var returnFleetMessage = message as ReturnFleetMessage;
            if (returnFleetMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            var timeToProcess = returnFleetMessage.ApproximateStartOfReturn - _dateTimeProvider.GetUtcNow();
            var shouldProcess = timeToProcess.TotalSeconds < 0;
            return shouldProcess;
        }

        public async Task<IEnumerable<Message>> ProcessAsync(Message message)
        {
            var returnFleetMessage = message as ReturnFleetMessage;
            if (returnFleetMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            _logger.Info($"Processing {returnFleetMessage} mission.");
            var allMissions = await _missionService.GetAllMissionsAsync();
            var attacksToCurrentPlanet = allMissions
                .Where(m => m.IsReturn == false &&
                            m.MissionType == MissionType.Attak &&
                            m.PlanetTo.Coordinates == returnFleetMessage.SaveMovement.PlanetFrom.Coordinates)
                .ToList();

            var utcNow = _dateTimeProvider.GetUtcNow();
            var timeToReturn = returnFleetMessage.SaveMovement.GetTimeToReturn(utcNow);
            var returnGreenZoneLeftBound = timeToReturn.TotalSeconds - 3;
            var returnGreenZoneRightBound = timeToReturn.TotalSeconds + 60;

            var canReturnRightNow = attacksToCurrentPlanet.All(m =>
                (m.ArrivalTimeUtc - utcNow).TotalSeconds < returnGreenZoneLeftBound ||
                (m.ArrivalTimeUtc - utcNow).TotalSeconds > returnGreenZoneRightBound
            );

            var resultMessages = new List<Message>();
            if (canReturnRightNow)
            {
                var returnMovement = await _fleetMovementService.ReturnFleetAsync(returnFleetMessage.SaveMovement.Id);
                var fleetArrivedMessage = new FleetArrivedMessage(returnMovement.ArrivalTimeUtc);
                resultMessages.Add(fleetArrivedMessage);
            }
            else
            {
                var dangerousAttackMission = attacksToCurrentPlanet
                    .Where(m =>
                        (m.ArrivalTimeUtc - utcNow).TotalSeconds >= returnGreenZoneLeftBound &&
                        (m.ArrivalTimeUtc - utcNow).TotalSeconds <= returnGreenZoneRightBound
                    )
                    .OrderBy(m => m.ArrivalTimeUtc)
                    .First();
                var approximateStartOfReturn = GetApproximateStartOfReturn(timeToReturn, dangerousAttackMission);
                var newMessage = new ReturnFleetMessage(returnFleetMessage.SaveMovement, approximateStartOfReturn);
                resultMessages.Add(newMessage);
            }
            return resultMessages;
        }

        private TimeSpan GetApproximateStartOfReturn(TimeSpan timeToReturn, Mission attackMission)
        {
            var utcNow = _dateTimeProvider.GetUtcNow();
            var timeToAttack = attackMission.ArrivalTimeUtc - utcNow;
            var diff = timeToAttack - timeToReturn;
            var approximateStartOfReturn = utcNow + TimeSpan.FromTicks(diff.Ticks / 2) + TimeSpan.FromSeconds(2);
            return approximateStartOfReturn;
        }
    }
}