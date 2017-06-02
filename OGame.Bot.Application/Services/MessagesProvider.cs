using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain;
using OGame.Bot.Domain.Services.Interfaces;

namespace OGame.Bot.Application.Services
{
    public class MessagesProvider : IMessagesProvider
    {
        private readonly IMissionService _missionService;

        public MessagesProvider(IMissionService missionService)
        {
            _missionService = missionService;
        }

        public async Task<IEnumerable<Message>> GetNewMessagesAsync()
        {
            var newMessages = new List<Message>();

            var attackMessages = await GetAttackMessagesAsync();
            newMessages.AddRange(attackMessages);

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
