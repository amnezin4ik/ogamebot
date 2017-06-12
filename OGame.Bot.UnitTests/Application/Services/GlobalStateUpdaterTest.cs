using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OGame.Bot.Application.MessageBus;
using OGame.Bot.Application.Services;
using OGame.Bot.Domain.Messages;

namespace OGame.Bot.UnitTests.Application.Services
{
    [TestFixture]
    public class GlobalStateUpdaterTest
    {
        [Test]
        public async Task Run_ShouldProcessMessages()
        {
            var messageServiceBusMock = new Mock<IMessageServiceBus>();

            var globalStateUpdater = new GlobalStateUpdater(messageServiceBusMock.Object);


            globalStateUpdater.Run();
            await Task.Delay(TimeSpan.FromMilliseconds(30));


            messageServiceBusMock.Verify(m => m.AddMessage(It.IsAny<UpdateStateMessage>()), Times.AtLeast(1));
            messageServiceBusMock.Verify(m => m.AddMessage(It.IsAny<Message>()), Times.AtLeast(1));
        }

        [Test]
        public void Run_ShouldThrowExceptionWhenCalledTwice()
        {
            var globalStateUpdater = new GlobalStateUpdater(null);
            globalStateUpdater.Run();
            Assert.Throws<InvalidOperationException>(() => globalStateUpdater.Run());
        }

        [Test]
        [Repeat(100)]
        public async Task StopAsync_ShouldStopProcessing()
        {
            var messageServiceBusMock = new Mock<IMessageServiceBus>();
            var globalStateUpdater = new GlobalStateUpdater(messageServiceBusMock.Object);

            globalStateUpdater.Run();

            Assert.IsTrue(globalStateUpdater.IsRunning);

            await globalStateUpdater.StopAsync();

            Assert.IsFalse(globalStateUpdater.IsRunning);
        }
    }
}