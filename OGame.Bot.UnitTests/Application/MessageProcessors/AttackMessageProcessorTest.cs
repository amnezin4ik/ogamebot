using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using OGame.Bot.Application.MessageProcessors.Implementations;
using OGame.Bot.Application.Messages;

namespace OGame.Bot.UnitTests.Application.MessageProcessors
{
    [TestFixture]
    public class AttackMessageProcessorTest
    {
        [Test]
        public void CanProcess_ShouldReturnTrueWithAttackMessageType()
        {
            var attackMessageProcessor = new AttackMessageProcessor(null, null, null, null, null, null, null);
            var messageToProcessMock = new Mock<Message>(MessageType.Attack);

            var canProcess = attackMessageProcessor.CanProcess(messageToProcessMock.Object);

            Assert.True(canProcess);
        }

        [Test]
        public void CanProcess_ShouldTrturnFalseWithOtherMessageType()
        {
            var attackMessageProcessor = new AttackMessageProcessor(null, null, null, null, null, null, null);

            var allMessageTypes = Enum.GetValues(typeof(MessageType)).Cast<MessageType>().ToList();
            var messageTypesWithoutAttack = allMessageTypes.Except(new[] {MessageType.Attack}).ToList();

            foreach (var messageType in messageTypesWithoutAttack)
            {
                var messageToProcessMock = new Mock<Message>(messageType);

                var canProcess = attackMessageProcessor.CanProcess(messageToProcessMock.Object);

                Assert.False(canProcess);
            }
        }
    }
}