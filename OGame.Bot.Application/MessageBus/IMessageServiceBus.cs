﻿using System.Threading;
using System.Threading.Tasks;
using OGame.Bot.Domain.Services.Messages;

namespace OGame.Bot.Application.MessageBus
{
    public interface IMessageServiceBus
    {
        bool IsRunning { get; }

        void AddMessage(Message message);

        void Run();

        Task StopAsync();
    }
}