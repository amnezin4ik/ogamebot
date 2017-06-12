using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using OGame.Bot.Application;
using OGame.Bot.Application.MessageProcessors.Implementations;
using OGame.Bot.Application.Messages;
using OGame.Bot.Domain;
using OGame.Bot.Domain.Services;
using OGame.Bot.Domain.Services.Interfaces;
using OGame.Bot.Modules.Common;
using OGame.Bot.Wpf;

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
        public void CanProcess_ShouldReturnFalseWithOtherMessageType()
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

        [Test]
        public void ShouldProcessRightNow_ShouldNotProcessMessagesCommingLessThenAMinute()
        {
            var utcNow = new TimeSpan(10, 1, 1, 1);

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock
                .Setup(m => m.GetUtcNow())
                .Returns(utcNow);

            var attackMessageProcessor = new AttackMessageProcessor(dateTimeProviderMock.Object, null, null, null, null, null, null);

            var missions = new[]
            {
                new Mission("") {ArrivalTimeUtc = utcNow.Add(new TimeSpan(-99, 0, 0, 0))},
                new Mission("") {ArrivalTimeUtc = utcNow},
                new Mission("") {ArrivalTimeUtc = utcNow.Add(new TimeSpan(0, 0, 0, 59))}
            };

            foreach (var mission in missions)
            {
                var attackMessage = new AttackMessage(mission);

                var shouldProcessRightNow = attackMessageProcessor.ShouldProcessRightNow(attackMessage);

                Assert.True(shouldProcessRightNow);
            }
        }

        [Test]
        public void ShouldProcessRightNow_ShouldNotProcessMessagesCommingInAMinuteOrMore()
        {
            var utcNow = new TimeSpan(10, 1, 1, 1);

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock
                .Setup(m => m.GetUtcNow())
                .Returns(utcNow);

            var attackMessageProcessor = new AttackMessageProcessor(dateTimeProviderMock.Object, null, null, null, null, null, null);

            var missions = new[]
            {
                new Mission("") {ArrivalTimeUtc = utcNow.Add(new TimeSpan(0, 0, 0, 60))},
                new Mission("") {ArrivalTimeUtc = utcNow.Add(new TimeSpan(99, 0, 0, 0))}
            };

            foreach (var mission in missions)
            {
                var attackMessage = new AttackMessage(mission);

                var shouldProcessRightNow = attackMessageProcessor.ShouldProcessRightNow(attackMessage);

                Assert.False(shouldProcessRightNow);
            }
        }

        [Test]
        public async Task ProcessAsync_ShouldNotGenerateAnyMessagesIfWeAreNotAttaked()
        {
            var userPlanetsServiceMock = new Mock<IUserPlanetsService>();
            userPlanetsServiceMock
                .Setup(m => m.IsItUserPlanetAsync(It.IsAny<Coordinates>()))
                .ReturnsAsync(false);

            var attackMessageProcessor = new AttackMessageProcessor(null, null, null, userPlanetsServiceMock.Object, null, null, null);

            var attackMessage = new AttackMessage(new Mission("")
            {
                PlanetTo = new MissionPlanet()
            });


            var messages = await attackMessageProcessor.ProcessAsync(attackMessage);


            userPlanetsServiceMock.Verify(m => m.IsItUserPlanetAsync(It.IsAny<Coordinates>()), Times.Once);
            Assert.AreEqual(0, messages.Count());
        }

        [Test]
        public async Task ProcessAsync_ShouldNotGenerateAnyMessagesIfAttackAlreadyNotExists()
        {
            var userPlanetsServiceMock = new Mock<IUserPlanetsService>();
            userPlanetsServiceMock
                .Setup(m => m.IsItUserPlanetAsync(It.IsAny<Coordinates>()))
                .ReturnsAsync(true);
            
            var missionServiceMock = new Mock<IMissionService>();
            missionServiceMock
                .Setup(m => m.IsFleetMovementStillExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var attackMessageProcessor = new AttackMessageProcessor(null, null, null, userPlanetsServiceMock.Object, missionServiceMock.Object, null, null);

            var attackMessage = new AttackMessage(new Mission("")
            {
                PlanetTo = new MissionPlanet()
            });


            var messages = await attackMessageProcessor.ProcessAsync(attackMessage);


            userPlanetsServiceMock.Verify(m => m.IsItUserPlanetAsync(It.IsAny<Coordinates>()), Times.Once);
            missionServiceMock.Verify(m => m.IsFleetMovementStillExistsAsync(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(0, messages.Count());
        }

        [Test]
        public async Task ProcessAsync_ShouldNotFindNearesnInactivePlanet()
        {
            var firstUserPlanet = new UserPlanet { Coordinates = new Coordinates(1, 1, 1) };
            var secondUserPlanet = new UserPlanet { Coordinates = new Coordinates(2, 2, 2) };

            var userPlanetsServiceMock = new Mock<IUserPlanetsService>();
            userPlanetsServiceMock
                .Setup(m => m.IsItUserPlanetAsync(It.IsAny<Coordinates>()))
                .ReturnsAsync(true);
            userPlanetsServiceMock
                .Setup(m => m.GetAllUserPlanetsAsync())
                .ReturnsAsync(new[] { firstUserPlanet, secondUserPlanet });

            Coordinates coordinates = null; 
            userPlanetsServiceMock
                .Setup(m => m.GetUserPlanetAsync(It.IsAny<Coordinates>()))
                .Callback<Coordinates>(c => coordinates = c)
                .ReturnsAsync(firstUserPlanet);

            var missionServiceMock = new Mock<IMissionService>();
            missionServiceMock
                .Setup(m => m.IsFleetMovementStillExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            
            var fleetServiceMock = new Mock<IFleetService>();
            fleetServiceMock
                .Setup(m => m.GetActivePlanetFleetAsync())
                .ReturnsAsync(new Fleet {ShipCells = new List<ShipCell>()});

            var galaxyServiceMock = new Mock<IGalaxyService>();
            
            var mapper = GetMapper();

            var attackMessageProcessor = new AttackMessageProcessor(null, fleetServiceMock.Object, galaxyServiceMock.Object, userPlanetsServiceMock.Object, missionServiceMock.Object, null, mapper);


            var attackMessage = new AttackMessage(new Mission("")
            {
                PlanetTo = new MissionPlanet { Coordinates = firstUserPlanet.Coordinates }
            });


            await attackMessageProcessor.ProcessAsync(attackMessage);

            
            userPlanetsServiceMock.Verify(m => m.GetAllUserPlanetsAsync(), Times.Once);
            galaxyServiceMock.Verify(m => m.GetNearestInactivePlanetAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task ProcessAsync_ShouldFindNearesnInactivePlanet()
        {
            var firstUserPlanet = new UserPlanet { Coordinates = new Coordinates(1, 1, 1) };

            var userPlanetsServiceMock = new Mock<IUserPlanetsService>();
            userPlanetsServiceMock
                .Setup(m => m.IsItUserPlanetAsync(It.IsAny<Coordinates>()))
                .ReturnsAsync(true);
            userPlanetsServiceMock
                .Setup(m => m.GetAllUserPlanetsAsync())
                .ReturnsAsync(new[] { firstUserPlanet });

            Coordinates coordinates = null;
            userPlanetsServiceMock
                .Setup(m => m.GetUserPlanetAsync(It.IsAny<Coordinates>()))
                .Callback<Coordinates>(c => coordinates = c)
                .ReturnsAsync(firstUserPlanet);

            var missionServiceMock = new Mock<IMissionService>();
            missionServiceMock
                .Setup(m => m.IsFleetMovementStillExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var fleetServiceMock = new Mock<IFleetService>();
            fleetServiceMock
                .Setup(m => m.GetActivePlanetFleetAsync())
                .ReturnsAsync(new Fleet { ShipCells = new List<ShipCell>() });

            var nearestInactivePlanet = new MissionPlanet { Coordinates = new Coordinates(2, 2, 2) };
            var galaxyServiceMock = new Mock<IGalaxyService>();
            galaxyServiceMock
                .Setup(m => m.GetNearestInactivePlanetAsync(It.IsAny<int>()))
                .ReturnsAsync(nearestInactivePlanet);

            var mapper = GetMapper();

            var attackMessageProcessor = new AttackMessageProcessor(null, fleetServiceMock.Object, galaxyServiceMock.Object, userPlanetsServiceMock.Object, missionServiceMock.Object, null, mapper);


            var attackMessage = new AttackMessage(new Mission("")
            {
                PlanetTo = new MissionPlanet { Coordinates = firstUserPlanet.Coordinates }
            });


            await attackMessageProcessor.ProcessAsync(attackMessage);


            userPlanetsServiceMock.Verify(m => m.GetAllUserPlanetsAsync(), Times.Once);
            galaxyServiceMock.Verify(m => m.GetNearestInactivePlanetAsync(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task ProcessAsync_ShouldNotSendFleetWithoutShips()
        {
            var firstUserPlanet = new UserPlanet { Coordinates = new Coordinates(1, 1, 1) };

            var userPlanetsServiceMock = new Mock<IUserPlanetsService>();
            userPlanetsServiceMock
                .Setup(m => m.IsItUserPlanetAsync(It.IsAny<Coordinates>()))
                .ReturnsAsync(true);
            
            userPlanetsServiceMock
                .Setup(m => m.GetUserPlanetAsync(It.IsAny<Coordinates>()))
                .ReturnsAsync(firstUserPlanet);

            var planetOverviewServiceMock = new Mock<IPlanetOverviewService>();

            var missionServiceMock = new Mock<IMissionService>();
            missionServiceMock
                .Setup(m => m.IsFleetMovementStillExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var fleetServiceMock = new Mock<IFleetService>();
            fleetServiceMock
                .Setup(m => m.GetActivePlanetFleetAsync())
                .ReturnsAsync(new Fleet { ShipCells = new List<ShipCell>() });

            var nearestInactivePlanet = new MissionPlanet { Coordinates = new Coordinates(2, 2, 2) };
            var galaxyServiceMock = new Mock<IGalaxyService>();
            galaxyServiceMock
                .Setup(m => m.GetNearestInactivePlanetAsync(It.IsAny<int>()))
                .ReturnsAsync(nearestInactivePlanet);

            var mapper = GetMapper();

            var attackMessageProcessor = new AttackMessageProcessor(null, fleetServiceMock.Object, galaxyServiceMock.Object, userPlanetsServiceMock.Object, missionServiceMock.Object, planetOverviewServiceMock.Object, mapper);

            var attackMessage = new AttackMessage(new Mission("")
            {
                PlanetTo = new MissionPlanet { Coordinates = firstUserPlanet.Coordinates }
            });


            await attackMessageProcessor.ProcessAsync(attackMessage);

            
            planetOverviewServiceMock.Verify(m => m.GetPlanetOverviewAsync(It.IsAny<UserPlanet>()), Times.Never);
            fleetServiceMock.Verify(m => m.SendFleetAsync(It.IsAny<Fleet>(), It.IsAny<Coordinates>(), It.IsAny<Coordinates>(), It.IsAny<MissionTarget>(), It.IsAny<MissionType>(), It.IsAny<FleetSpeed>(), It.IsAny<Resources>()), Times.Never);
        }

        [Test]
        public async Task ProcessAsync_ShouldSendFleetWithAllResources()
        {
            var firstUserPlanet = new UserPlanet { Coordinates = new Coordinates(1, 1, 1) };

            var userPlanetsServiceMock = new Mock<IUserPlanetsService>();
            userPlanetsServiceMock
                .Setup(m => m.IsItUserPlanetAsync(It.IsAny<Coordinates>()))
                .ReturnsAsync(true);

            userPlanetsServiceMock
                .Setup(m => m.GetUserPlanetAsync(It.IsAny<Coordinates>()))
                .ReturnsAsync(firstUserPlanet);

            var planetResources = new Resources { Crystal = 100, Deuterium = 200, Metal = 300 };
            var planetOverviewServiceMock = new Mock<IPlanetOverviewService>();
            planetOverviewServiceMock
                .Setup(m => m.GetPlanetOverviewAsync(It.IsAny<UserPlanet>()))
                .ReturnsAsync(new PlanetOverview { Resources = planetResources });

            var missionServiceMock = new Mock<IMissionService>();
            missionServiceMock
                .Setup(m => m.IsFleetMovementStillExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var fleetServiceMock = new Mock<IFleetService>();
            fleetServiceMock
                .Setup(m => m.GetActivePlanetFleetAsync())
                .ReturnsAsync(new Fleet
                {
                    ShipCells = new List<ShipCell>
                    {
                        new ShipCell
                        {
                            Count = 10,
                            Ship = new Ship
                            {
                                ShipType = ShipType.LargeTransport
                            }
                        }
                    }
                });

            var nearestInactivePlanet = new MissionPlanet { Coordinates = new Coordinates(2, 2, 2) };
            var galaxyServiceMock = new Mock<IGalaxyService>();
            galaxyServiceMock
                .Setup(m => m.GetNearestInactivePlanetAsync(It.IsAny<int>()))
                .ReturnsAsync(nearestInactivePlanet);

            var mapper = GetMapper();

            var attackMessageProcessor = new AttackMessageProcessor(null, fleetServiceMock.Object, galaxyServiceMock.Object, userPlanetsServiceMock.Object, missionServiceMock.Object, planetOverviewServiceMock.Object, mapper);

            var attackMessage = new AttackMessage(new Mission("")
            {
                PlanetTo = new MissionPlanet { Coordinates = firstUserPlanet.Coordinates }
            });


            await attackMessageProcessor.ProcessAsync(attackMessage);


            planetOverviewServiceMock.Verify(m => m.GetPlanetOverviewAsync(It.IsAny<UserPlanet>()), Times.Once);
            fleetServiceMock.Verify(m => m.SendFleetAsync(It.IsAny<Fleet>(), It.IsAny<Coordinates>(), It.IsAny<Coordinates>(), It.IsAny<MissionTarget>(), It.IsAny<MissionType>(), It.IsAny<FleetSpeed>(), planetResources), Times.Once);
        }

        [Test]
        public async Task ProcessAsync_ShouldReturnReturnFleetMessage()
        {
            var firstUserPlanet = new UserPlanet { Coordinates = new Coordinates(1, 1, 1) };

            var userPlanetsServiceMock = new Mock<IUserPlanetsService>();
            userPlanetsServiceMock
                .Setup(m => m.IsItUserPlanetAsync(It.IsAny<Coordinates>()))
                .ReturnsAsync(true);

            userPlanetsServiceMock
                .Setup(m => m.GetUserPlanetAsync(It.IsAny<Coordinates>()))
                .ReturnsAsync(firstUserPlanet);

            var planetResources = new Resources { Crystal = 100, Deuterium = 200, Metal = 300 };
            var planetOverviewServiceMock = new Mock<IPlanetOverviewService>();
            planetOverviewServiceMock
                .Setup(m => m.GetPlanetOverviewAsync(It.IsAny<UserPlanet>()))
                .ReturnsAsync(new PlanetOverview { Resources = planetResources });

            var missionServiceMock = new Mock<IMissionService>();
            missionServiceMock
                .Setup(m => m.IsFleetMovementStillExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var fleetServiceMock = new Mock<IFleetService>();
            fleetServiceMock
                .Setup(m => m.GetActivePlanetFleetAsync())
                .ReturnsAsync(new Fleet
                {
                    ShipCells = new List<ShipCell>
                    {
                        new ShipCell
                        {
                            Count = 10,
                            Ship = new Ship
                            {
                                ShipType = ShipType.LargeTransport
                            }
                        }
                    }
                });
            fleetServiceMock
                .Setup(m => m.SendFleetAsync(It.IsAny<Fleet>(), It.IsAny<Coordinates>(), It.IsAny<Coordinates>(), It.IsAny<MissionTarget>(), It.IsAny<MissionType>(), It.IsAny<FleetSpeed>(), It.IsAny<Resources>()))
                .ReturnsAsync(new FleetMovement());

            var nearestInactivePlanet = new MissionPlanet { Coordinates = new Coordinates(2, 2, 2) };
            var galaxyServiceMock = new Mock<IGalaxyService>();
            galaxyServiceMock
                .Setup(m => m.GetNearestInactivePlanetAsync(It.IsAny<int>()))
                .ReturnsAsync(nearestInactivePlanet);

            var mapper = GetMapper();

            var attackMessageProcessor = new AttackMessageProcessor(null, fleetServiceMock.Object, galaxyServiceMock.Object, userPlanetsServiceMock.Object, missionServiceMock.Object, planetOverviewServiceMock.Object, mapper);

            var attackMessage = new AttackMessage(new Mission("")
            {
                PlanetTo = new MissionPlanet { Coordinates = firstUserPlanet.Coordinates }
            });


            await attackMessageProcessor.ProcessAsync(attackMessage);


            planetOverviewServiceMock.Verify(m => m.GetPlanetOverviewAsync(It.IsAny<UserPlanet>()), Times.Once);
            fleetServiceMock.Verify(m => m.SendFleetAsync(It.IsAny<Fleet>(), It.IsAny<Coordinates>(), It.IsAny<Coordinates>(), It.IsAny<MissionTarget>(), It.IsAny<MissionType>(), It.IsAny<FleetSpeed>(), planetResources), Times.Once);
        }



        private IMapper GetMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(WpfMappingProfile));
                cfg.AddProfile(typeof(ApplicationMappingProfile));
                cfg.AddProfile(typeof(DomainMappingProfile));
            });
            var mapper = mapperConfiguration.CreateMapper();
            return mapper;
        }
    }
}