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
        private readonly Queue<Message> _messagesQueue;

        public MessageServiceBus(IMessageProcessorFactory messageProcessorFactory)
        {
            _messageProcessorFactory = messageProcessorFactory;
            _messagesQueue = new Queue<Message>();
        }

        public void EnqueueMessage(Message message)
        {
            _messagesQueue.Enqueue(message);
        }

        public async Task RunProcessing(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_messagesQueue.Any())
                {
                    var message = _messagesQueue.Dequeue();
                    var messageProcessor = _messageProcessorFactory.GetMessageProcessor(message);
                    if (messageProcessor.ShouldProcessRightNow(message))
                    {
                        await messageProcessor.Process(message);
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