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

        public MessageServiceBus(IMessageProcessorFactory messageProcessorFactory, IMessagesComparer messagesComparer)
        {
            _messageProcessorFactory = messageProcessorFactory;
            _messagesComparer = messagesComparer;
            _messagesQueue = new Queue<Message>();
        }

        public void Add(Message message)
        {
            if (!_messagesQueue.Contains(message, _messagesComparer))
            {
                _messagesQueue.Enqueue(message);
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
                        await messageProcessor.ProcessAsync(message);
                    }
                    else
                    {
                        _messagesQueue.Enqueue(message);
                    }
                }
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}