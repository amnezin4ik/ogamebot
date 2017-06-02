﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using OGame.Bot.Application.MessageProcessors;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.Application.MessageBus
{
    public sealed class MessageServiceBus : IMessageServiceBus
    {
        private readonly Logger _logger = LogManager.GetLogger(nameof(MessageServiceBus));
        private readonly IMessageProcessorFactory _messageProcessorFactory;
        private readonly IMessagesComparer _messagesComparer;
        private readonly ConcurrentQueue<Message> _messagesQueue;
        private readonly object _enqueueSyncRoot;
        private Task _runTask;
        private CancellationTokenSource _runTaskCancellationTokenSource;

        public MessageServiceBus(IMessageProcessorFactory messageProcessorFactory, IMessagesComparer messagesComparer)
        {
            _messageProcessorFactory = messageProcessorFactory;
            _messagesComparer = messagesComparer;
            _messagesQueue = new ConcurrentQueue<Message>();
            _enqueueSyncRoot = new object();
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }

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

        public void Run()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Run method was already called");
            }
            IsRunning = true;

            _runTaskCancellationTokenSource = new CancellationTokenSource();
            _runTask = Task.Run(async () =>
            {
                while (!_runTaskCancellationTokenSource.IsCancellationRequested)
                {
                    Message message;
                    if (_messagesQueue.TryDequeue(out message))
                    {
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
                }
            }, _runTaskCancellationTokenSource.Token);
        }

        public async Task StopAsync()
        {
            if (IsRunning)
            {
                _runTaskCancellationTokenSource.Cancel();
                try
                {
                    if (_runTask.Status != TaskStatus.Canceled &&
                        _runTask.Status != TaskStatus.WaitingForActivation)
                    {
                        await _runTask;
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    throw;
                }
                finally
                {
                    _runTaskCancellationTokenSource.Dispose();
                }
            }
            IsRunning = false;
        }
    }
}