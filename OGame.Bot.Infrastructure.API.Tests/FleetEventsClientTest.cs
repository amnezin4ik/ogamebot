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

namespace OGame.Bot.Infrastructure.API.Tests
{
    [TestFixture]
    public class FleetEventsClientTest
    {
        [Test]
        public async Task ShouldReturnTransportFleetEvent()
        {
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(m => m.GetHttpClient(It.IsAny<HttpClientHandler>()))
                .Returns(new HttpClient());

            var testResponce = GetTransportTestResponce();
            var httpHelperMock = new Mock<IHttpHelper>();
            httpHelperMock
                .Setup(m => m.GetStringAsync(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .ReturnsAsync(testResponce);

            var htmlParser = new HtmlParser();
            var fleetEventsClient = new MissionClient(httpClientFactoryMock.Object, httpHelperMock.Object, htmlParser);
            var cookieContainer = new CookieContainer();
            var sessionData = new SessionData(cookieContainer);


            var fleetEvents = await fleetEventsClient.GetAllMissionsAsync(sessionData);


            Assert.AreEqual(2, fleetEvents.Count());
            var firstEvent = fleetEvents.ElementAt(0);
            Assert.AreEqual("eventRow-1336655", firstEvent.Id);
            Assert.AreEqual(MissionType.Transport, firstEvent.MissionType);
            Assert.AreEqual(1494962729, firstEvent.ArrivalTimeUtc.TotalSeconds);
            Assert.AreEqual("p1", firstEvent.PlanetFrom.PlanetName);
            Assert.AreEqual(1, firstEvent.PlanetFrom.PlanetCoordinates.Galaxy);
            Assert.AreEqual(279, firstEvent.PlanetFrom.PlanetCoordinates.System);
            Assert.AreEqual(6, firstEvent.PlanetFrom.PlanetCoordinates.Position);
            Assert.AreEqual("Главная планета", firstEvent.PlanetTo.PlanetName);
            Assert.AreEqual(1, firstEvent.PlanetTo.PlanetCoordinates.Galaxy);
            Assert.AreEqual(279, firstEvent.PlanetTo.PlanetCoordinates.System);
            Assert.AreEqual(10, firstEvent.PlanetTo.PlanetCoordinates.Position);

            var secondEvent = fleetEvents.ElementAt(1);
            Assert.AreEqual("eventRow-1336656", secondEvent.Id);
            Assert.AreEqual(MissionType.Transport, secondEvent.MissionType);
            Assert.AreEqual(1494963376, secondEvent.ArrivalTimeUtc.TotalSeconds);
            Assert.AreEqual("p1", secondEvent.PlanetFrom.PlanetName);
            Assert.AreEqual(1, secondEvent.PlanetFrom.PlanetCoordinates.Galaxy);
            Assert.AreEqual(279, secondEvent.PlanetFrom.PlanetCoordinates.System);
            Assert.AreEqual(6, secondEvent.PlanetFrom.PlanetCoordinates.Position);
            Assert.AreEqual("Главная планета", secondEvent.PlanetTo.PlanetName);
            Assert.AreEqual(1, secondEvent.PlanetTo.PlanetCoordinates.Galaxy);
            Assert.AreEqual(279, secondEvent.PlanetTo.PlanetCoordinates.System);
            Assert.AreEqual(10, secondEvent.PlanetTo.PlanetCoordinates.Position);
        }

        private string GetTransportTestResponce()
        {
            var testResponceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "EventList_TransportEvents.txt");
            var testResponce = File.ReadAllText(testResponceFilePath);
            return testResponce;
        }
    }
}
