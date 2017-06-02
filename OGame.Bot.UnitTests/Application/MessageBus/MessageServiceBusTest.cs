using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Application.MessageProcessors;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain;

namespace OGame.Bot.UnitTests.Application.MessageBus
{
    [TestFixture]
    public class MessageServiceBusTest
    {
        [Test]
        public async Task Run_ShouldProcessMessage()
        {
            var messageProcessorMock = new Mock<IMessageProcessor>();
            messageProcessorMock
                .Setup(m => m.ShouldProcessRightNow(It.IsAny<Message>()))
                .Returns(true);
            messageProcessorMock
                .Setup(m => m.ProcessAsync(It.IsAny<Message>()))
                .ReturnsAsync(new List<Message>());

            var messageProcessorFactoryMock = new Mock<IMessageProcessorFactory>();
            messageProcessorFactoryMock
                .Setup(m => m.GetMessageProcessor(It.IsAny<Message>()))
                .Returns(messageProcessorMock.Object);

            var messagesComparerMock = new Mock<IMessagesComparer>();

            var messageToProcess = new AttackMessage(new Mission(""));
            var messageServiceBus = new MessageServiceBus(messageProcessorFactoryMock.Object, messagesComparerMock.Object);
            messageServiceBus.AddMessage(messageToProcess);

          
            messageServiceBus.Run();
            await Task.Delay(TimeSpan.FromMilliseconds(30));


            messageProcessorMock.Verify(m => m.ShouldProcessRightNow(It.IsAny<Message>()), Times.Once);
            messageProcessorMock.Verify(m => m.ProcessAsync(It.IsAny<Message>()), Times.Once);
        }

        [Test]
        public void Run_ShouldThrowExceptionWhenCalledTwice()
        {
            var messageProcessorFactoryMock = new Mock<IMessageProcessorFactory>();
            var messagesComparerMock = new Mock<IMessagesComparer>();
            var messageServiceBus = new MessageServiceBus(messageProcessorFactoryMock.Object, messagesComparerMock.Object);

            messageServiceBus.Run();

            Assert.Throws<InvalidOperationException>(() => messageServiceBus.Run());
        }

        [Test]
        public async Task StopAsync_ShouldStopProcessing()
        {
            var messageProcessorMock = new Mock<IMessageProcessor>();
            var messageProcessorFactoryMock = new Mock<IMessageProcessorFactory>();
            messageProcessorFactoryMock
                .Setup(m => m.GetMessageProcessor(It.IsAny<Message>()))
                .Returns(messageProcessorMock.Object);
            var messagesComparerMock = new Mock<IMessagesComparer>();
            var messageServiceBus = new MessageServiceBus(messageProcessorFactoryMock.Object, messagesComparerMock.Object);

            messageServiceBus.Run();
            Assert.IsTrue(messageServiceBus.IsRunning);


            await messageServiceBus.StopAsync();
            var messageToProcess = new AttackMessage(new Mission(""));
            messageServiceBus.AddMessage(messageToProcess);
            await Task.Delay(TimeSpan.FromMilliseconds(30));


            Assert.IsFalse(messageServiceBus.IsRunning);
            messageProcessorFactoryMock.Verify(m => m.GetMessageProcessor(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public async Task AddMessage_ShouldAddMessagesToMessageQueue()
        {
            var messageProcessorMock = new Mock<IMessageProcessor>();
            messageProcessorMock
                .Setup(m => m.ShouldProcessRightNow(It.IsAny<Message>()))
                .Returns(true);
            messageProcessorMock
                .Setup(m => m.ProcessAsync(It.IsAny<Message>()))
                .ReturnsAsync(new List<Message>());

            var messageProcessorFactoryMock = new Mock<IMessageProcessorFactory>();
            messageProcessorFactoryMock
                .Setup(m => m.GetMessageProcessor(It.IsAny<Message>()))
                .Returns(messageProcessorMock.Object);

            var messagesComparerMock = new Mock<IMessagesComparer>();
            var messageServiceBus = new MessageServiceBus(messageProcessorFactoryMock.Object, messagesComparerMock.Object);
            messageServiceBus.Run();


            messageServiceBus.AddMessage(new AttackMessage(new Mission("")));
            messageServiceBus.AddMessage(new ReturnFleetMessage(new Mission(""), TimeSpan.MinValue));
            messageServiceBus.AddMessage(new UpdateSessionDataMessage());
            await Task.Delay(TimeSpan.FromMilliseconds(50));


            messageProcessorMock.Verify(m => m.ShouldProcessRightNow(It.IsAny<Message>()), Times.Exactly(3));
            messageProcessorMock.Verify(m => m.ProcessAsync(It.IsAny<Message>()), Times.Exactly(3));
        }

        [Test]
        public async Task AddMessage_ShouldNotAddSameMessages()
        {
            var messageProcessorMock = new Mock<IMessageProcessor>();
            messageProcessorMock
                .Setup(m => m.ShouldProcessRightNow(It.IsAny<Message>()))
                .Returns(true);
            messageProcessorMock
                .Setup(m => m.ProcessAsync(It.IsAny<Message>()))
                .ReturnsAsync(new List<Message>());

            var messageProcessorFactoryMock = new Mock<IMessageProcessorFactory>();
            messageProcessorFactoryMock
                .Setup(m => m.GetMessageProcessor(It.IsAny<Message>()))
                .Returns(messageProcessorMock.Object);

            var messagesComparer = new MessagesComparer();
            var messageServiceBus = new MessageServiceBus(messageProcessorFactoryMock.Object, messagesComparer);


            messageServiceBus.AddMessage(new AttackMessage(new Mission("2134")));
            messageServiceBus.AddMessage(new AttackMessage(new Mission("2134")));
            messageServiceBus.AddMessage(new UpdateSessionDataMessage());
            messageServiceBus.AddMessage(new UpdateSessionDataMessage());
            messageServiceBus.AddMessage(new UpdateSessionDataMessage());
            messageServiceBus.Run();
            await Task.Delay(TimeSpan.FromMilliseconds(50));


            messageProcessorMock.Verify(m => m.ShouldProcessRightNow(It.IsAny<Message>()), Times.Exactly(2));
            messageProcessorMock.Verify(m => m.ProcessAsync(It.IsAny<Message>()), Times.Exactly(2));
        }
    }
}