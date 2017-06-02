using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Application.Messages;
using OGame.Bot.Application.Services;
using OGame.Bot.Domain;

namespace OGame.Bot.UnitTests.Application.Services
{
    [TestFixture]
    public class GlobalStateUpdaterTest
    {
        [Test]
        public async Task Run_ShouldProcessMessages()
        {
            var messageServiceBusMock = new Mock<IMessageServiceBus>();

            var mission = new Mission("") {MissionType = MissionType.Attak};
            var newMissions = new[]
            {
                new AttackMessage(mission),
                new AttackMessage(mission)
            };

            var messagesProviderMock = new Mock<IMessagesProvider>();
            messagesProviderMock
                .Setup(m => m.GetNewMessagesAsync())
                .ReturnsAsync(newMissions);

            var globalStateUpdater = new GlobalStateUpdater(messageServiceBusMock.Object, messagesProviderMock.Object);


            globalStateUpdater.Run();
            await Task.Delay(TimeSpan.FromMilliseconds(30));


            messageServiceBusMock.Verify(m => m.AddMessage(It.IsAny<Message>()), Times.Exactly(2));
        }

        [Test]
        public void Run_ShouldThrowExceptionWhenCalledTwice()
        {
            var globalStateUpdater = new GlobalStateUpdater(null, null);
            globalStateUpdater.Run();
            Assert.Throws<InvalidOperationException>(() => globalStateUpdater.Run());
        }

        [Test]
        [Repeat(100)]
        public async Task StopAsync_ShouldStopProcessing()
        {
            var messageServiceBusMock = new Mock<IMessageServiceBus>();
            var messagesProviderMock = new Mock<IMessagesProvider>();
            var globalStateUpdater = new GlobalStateUpdater(messageServiceBusMock.Object, messagesProviderMock.Object);

            globalStateUpdater.Run();

            Assert.IsTrue(globalStateUpdater.IsRunning);

            await globalStateUpdater.StopAsync();

            Assert.IsFalse(globalStateUpdater.IsRunning);
        }
    }
}