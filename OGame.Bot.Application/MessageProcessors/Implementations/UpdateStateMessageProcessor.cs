using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Application.MessageProcessors.Interfaces;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain;
using OGame.Bot.Domain.Services.Interfaces;

namespace OGame.Bot.Application.MessageProcessors.Implementations
{
    public class UpdateStateMessageProcessor : IUpdateStateMessageProcessor
    {
        private readonly ILogger _logger = LogManager.GetLogger(nameof(UpdateStateMessageProcessor));
        private readonly IMissionService _missionService;

        public UpdateStateMessageProcessor(IMissionService missionService)
        {
            _missionService = missionService;
        }

        public bool CanProcess(Message message)
        {
            return message.MessageType == MessageType.UpdateState;
        }

        public bool ShouldProcessRightNow(Message message)
        {
            var updateStateMessage = message as UpdateStateMessage;
            if (updateStateMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            return true;
        }

        public async Task<IEnumerable<Message>> ProcessAsync(Message message)
        {
            _logger.Info("44");
            var updateStateMessage = message as UpdateStateMessage;
            if (updateStateMessage == null)
            {
                throw new NotSupportedException($"Can't process message with \"{message.MessageType}\" message type");
            }

            var newMessages = await GetNewMessagesAsync(updateStateMessage.MessageTypesToUpdate);
            return newMessages;
        }

        private async Task<IEnumerable<Message>> GetNewMessagesAsync(IEnumerable<MessageType> messageTypes)
        {
            var newMessages = new List<Message>();

            if (messageTypes.Contains(MessageType.Attack))
            {
                var attackMessages = await GetAttackMessagesAsync();
                newMessages.AddRange(attackMessages);
            }

            if (messageTypes.Contains(MessageType.UpdateSessionData))
            {
                var updateSessionDataMessage = new UpdateSessionDataMessage();
                newMessages.Add(updateSessionDataMessage);
            }

            return newMessages;
        }

        private async Task<IEnumerable<Message>> GetAttackMessagesAsync()
        {
            var attackMissions = await _missionService.GetMissionsAsync(MissionType.Attak);
            var attackMessages = attackMissions.Select(am => new AttackMessage(am)).ToList();
            return attackMessages;
        }
    }
}