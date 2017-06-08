using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.Application.MessageProcessors
{
    public class ReturnFleetMessageProcessor : IReturnFleetMessageProcessor
    {
        private readonly IMissionService _missionService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReturnFleetMessageProcessor(IMissionService missionService, IDateTimeProvider dateTimeProvider)
        {
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


            //var allMissions = await _missionService.GetAllMissionsAsync();
            //var attacsToCurrentPlanet = allMissions
            //    .Where(m => m.IsReturn == false && 
            //                m.MissionType == MissionType.Attak &&
            //                m.PlanetTo.Coordinates == returnFleetMessage.SaveMission.PlanetFrom.Coordinates)
            //    .ToList();

            //var timeToReturn = returnFleetMessage.SaveMission.

            ////TODO: move this method to another service (FleetMovementService)
            //await _missionService.ReturnFleetAsync(returnFleetMessage.SaveMission.Id);
            return new List<Message>();
        }
    }
}