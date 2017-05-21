using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OGame.Bot.Application.MessageProcessors;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.MessageBus
{
    public sealed class MessageServiceBus : IMessageServiceBus
    {
        private readonly IMessageProcessorFactory _messageProcessorFactory;
        private readonly IMessagesComparer _messagesComparer;
        private readonly Queue<Message> _messagesQueue;
        private readonly int _processingReRunDelayInSeconds;
        private readonly object _enqueueSyncRoot;

        public MessageServiceBus(IMessageProcessorFactory messageProcessorFactory, IMessagesComparer messagesComparer)
        {
            _messageProcessorFactory = messageProcessorFactory;
            _messagesComparer = messagesComparer;
            _messagesQueue = new Queue<Message>();
            _processingReRunDelayInSeconds = 1;
            _enqueueSyncRoot = new object();
        }

        public void AddMessage(Message message)
        {
            if (!_messagesQueue.Contains(message, _messagesComparer))
            {
                lock (_enqueueSyncRoot)
                {
                    if (!_messagesQueue.Contains(message, _messagesComparer))
                    {
                        _messagesQueue.Enqueue(message);
                    }
                }
            }
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_messagesQueue.Any())
                {
                    var message = _messagesQueue.Dequeue();
                    var messageProcessor = _messageProcessorFactory.GetMessageProcessor(message);
                    if (messageProcessor.ShouldProcessRightNow(message))
                    {
                        var postProcessMessages = await messageProcessor.ProcessAsync(message);
                        foreach (var postProcessMessage in postProcessMessages)
                        {
                            AddMessage(postProcessMessage);
                        }
                    }
                    else
                    {
                        AddMessage(message);
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(_processingReRunDelayInSeconds), cancellationToken);
            }
        }
    }
}