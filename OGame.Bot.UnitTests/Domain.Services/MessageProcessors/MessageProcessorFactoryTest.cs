using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using OGame.Bot.Domain.Messages;
using OGame.Bot.Domain.Services.MessageProcessors;
using OGame.Bot.Domain.Services.MessageProcessors.Implementations;

namespace OGame.Bot.UnitTests.Domain.Services.MessageProcessors
{
    [TestFixture]
    public class MessageProcessorFactoryTest
    {
        [Test]
        public void ShouldReturnMessageProcessor()
        {
            var attackMessageProcessorMock = new AttackMessageProcessor(null, null, null, null, null, null, null);
            var updateSessionDataMessageProcessor = new UpdateSessionDataMessageProcessor(null);
            var returnFleetMessageProcessor = new ReturnFleetMessageProcessor(null, null, null);
            var fleetArrivedMessageProcessor = new FleetArrivedMessageProcessor(null);
            var updateStateMessageProcessor = new UpdateStateMessageProcessor(null);
            var transportMessageProcessor = new TransportMessageProcessor();

            var messageProcessorFactory = new MessageProcessorFactory(
                attackMessageProcessorMock, 
                updateSessionDataMessageProcessor, 
                returnFleetMessageProcessor, 
                fleetArrivedMessageProcessor, 
                updateStateMessageProcessor,
                transportMessageProcessor);

            var allMessageTypes = Enum.GetValues(typeof(MessageType)).Cast<MessageType>().ToList();

            foreach (var messageType in allMessageTypes)
            {
                var message = new Mock<Message>(messageType);

                messageProcessorFactory.GetMessageProcessor(message.Object);
            }
        }
    }
}