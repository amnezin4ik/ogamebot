using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Moq;
using NUnit.Framework;
using OGame.Bot.Infrastructure.API.APIClients;
using OGame.Bot.Infrastructure.API.Dto;
using OGame.Bot.Infrastructure.API.Helpers;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.Infrastructure.API.Tests
{
    [TestFixture]
    public class FleetMovementClientTest
    {
        [Test]
        public async Task ShouldReturnTransportFleetEvent()
        {
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(m => m.GetHttpClient(It.IsAny<SessionData>()))
                .Returns(new HttpClient());

            var testResponce = GetTransportTestResponce();
            var httpHelperMock = new Mock<IHttpHelper>();
            httpHelperMock
                .Setup(m => m.GetStringAsync(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .ReturnsAsync(testResponce);

            var htmlParser = new HtmlParser();
            var coordinatesParser = new CoordinatesParser();
            var dateTimeProvider = new SystemDateTimeProvider();
            var fleetMovementClient = new FleetMovementClient(httpClientFactoryMock.Object, httpHelperMock.Object, htmlParser, coordinatesParser, dateTimeProvider);
            var cookieContainer = new CookieContainer();
            var sessionData = new SessionData(cookieContainer);


            var fleetEvents = await fleetMovementClient.GetAllFleetMovementsAsync(sessionData);
            

            Assert.AreEqual(2, fleetEvents.Count());
            var firstEvent = fleetEvents.ElementAt(0);
            Assert.AreEqual("861226", firstEvent.Id);
            Assert.AreEqual(MissionType.Transport, firstEvent.MissionType);
            Assert.AreEqual(63632516196, firstEvent.ArrivalTimeUtc.TotalSeconds);
            Assert.AreEqual(63632517528, firstEvent.ReturnTimeUtc.TotalSeconds);
            Assert.AreEqual("p1", firstEvent.PlanetFrom.Name);
            Assert.AreEqual(1, firstEvent.PlanetFrom.Coordinates.Galaxy);
            Assert.AreEqual(279, firstEvent.PlanetFrom.Coordinates.System);
            Assert.AreEqual(6, firstEvent.PlanetFrom.Coordinates.Position);
            Assert.AreEqual("p2", firstEvent.PlanetTo.Name);
            Assert.AreEqual(1, firstEvent.PlanetTo.Coordinates.Galaxy);
            Assert.AreEqual(299, firstEvent.PlanetTo.Coordinates.System);
            Assert.AreEqual(9, firstEvent.PlanetTo.Coordinates.Position);
            Assert.IsFalse(firstEvent.IsReturn);

            var secondEvent = fleetEvents.ElementAt(1);
            Assert.AreEqual("861207", secondEvent.Id);
            Assert.AreEqual(MissionType.Transport, secondEvent.MissionType);
            Assert.AreEqual(63632516805, secondEvent.ArrivalTimeUtc.TotalSeconds);
            Assert.AreEqual(0, secondEvent.ReturnTimeUtc.TotalSeconds);
            Assert.AreEqual("p1", secondEvent.PlanetFrom.Name);
            Assert.AreEqual(1, secondEvent.PlanetFrom.Coordinates.Galaxy);
            Assert.AreEqual(279, secondEvent.PlanetFrom.Coordinates.System);
            Assert.AreEqual(6, secondEvent.PlanetFrom.Coordinates.Position);
            Assert.AreEqual("p2", secondEvent.PlanetTo.Name);
            Assert.AreEqual(1, secondEvent.PlanetTo.Coordinates.Galaxy);
            Assert.AreEqual(299, secondEvent.PlanetTo.Coordinates.System);
            Assert.AreEqual(9, secondEvent.PlanetTo.Coordinates.Position);
            Assert.IsTrue(secondEvent.IsReturn);
        }

        private string GetTransportTestResponce()
        {
            var testResponceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "FleetMovement.txt");
            var testResponce = File.ReadAllText(testResponceFilePath);
            return testResponce;
        }
    }
}