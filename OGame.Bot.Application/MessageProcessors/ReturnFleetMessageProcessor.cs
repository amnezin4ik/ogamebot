using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.Application.MessageProcessors
{
    public class ReturnFleetMessageProcessor : IReturnFleetMessageProcessor
    {
        private readonly IFleetMovementService _fleetMovementService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReturnFleetMessageProcessor(IFleetMovementService fleetMovementService, IDateTimeProvider dateTimeProvider)
        {
            _fleetMovementService = fleetMovementService;
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

            //TODO: add logic to protect returning under second or another enemy fleet wave.
            //var allMissions = await _missionService.GetAllMissionsAsync();
            //var attacsToCurrentPlanet = allMissions
            //    .Where(m => m.IsReturn == false && 
            //                m.MissionType == MissionType.Attak &&
            //                m.PlanetTo.Coordinates == returnFleetMessage.SaveMovement.PlanetFrom.Coordinates)
            //    .ToList();

            //var timeToReturn = returnFleetMessage.SaveMovement.

            //TODO: it should return arrival time
            await _fleetMovementService.ReturnFleetAsync(returnFleetMessage.SaveMovement.Id);

            //TODO: return new message with arrivalTime, then process it by new message processor to be sure, that after arrival we do not meet another wave of enemy fleet
            return new List<Message>();
        }
    }
}