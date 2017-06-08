﻿using System;
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
    public class MissionClientTest
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

            var systemDateTimeProvider = new SystemDateTimeProvider();

            var htmlParser = new HtmlParser();
            var coordinatesParser = new CoordinatesParser();
            var dateTimeProvider = new SystemDateTimeProvider();
            var missionClient = new MissionClient(httpClientFactoryMock.Object, httpHelperMock.Object, htmlParser, coordinatesParser, dateTimeProvider);
            var cookieContainer = new CookieContainer();
            var sessionData = new SessionData(cookieContainer);


            var fleetEvents = await missionClient.GetAllMissionsAsync(sessionData);
            

            Assert.AreEqual(2, fleetEvents.Count());
            var firstEvent = fleetEvents.ElementAt(0);
            Assert.AreEqual("eventRow-1336655", firstEvent.Id);
            Assert.AreEqual(MissionType.Transport, firstEvent.MissionType);
            var arrivalTimeUtc1 = systemDateTimeProvider.GetOGameUtcOffset() + TimeSpan.FromSeconds(1494962729);
            Assert.AreEqual(arrivalTimeUtc1.TotalSeconds, firstEvent.ArrivalTimeUtc.TotalSeconds);
            Assert.AreEqual("p1", firstEvent.PlanetFrom.Name);
            Assert.AreEqual(1, firstEvent.PlanetFrom.Coordinates.Galaxy);
            Assert.AreEqual(279, firstEvent.PlanetFrom.Coordinates.System);
            Assert.AreEqual(6, firstEvent.PlanetFrom.Coordinates.Position);
            Assert.AreEqual("Главная планета", firstEvent.PlanetTo.Name);
            Assert.AreEqual(1, firstEvent.PlanetTo.Coordinates.Galaxy);
            Assert.AreEqual(279, firstEvent.PlanetTo.Coordinates.System);
            Assert.AreEqual(10, firstEvent.PlanetTo.Coordinates.Position);
            Assert.IsFalse(firstEvent.IsReturn);

            var secondEvent = fleetEvents.ElementAt(1);
            Assert.AreEqual("eventRow-1336656", secondEvent.Id);
            Assert.AreEqual(MissionType.Transport, secondEvent.MissionType);
            var arrivalTimeUtc2 = systemDateTimeProvider.GetOGameUtcOffset() + TimeSpan.FromSeconds(1494963376);
            Assert.AreEqual(arrivalTimeUtc2.TotalSeconds, secondEvent.ArrivalTimeUtc.TotalSeconds);
            Assert.AreEqual("p1", secondEvent.PlanetFrom.Name);
            Assert.AreEqual(1, secondEvent.PlanetFrom.Coordinates.Galaxy);
            Assert.AreEqual(279, secondEvent.PlanetFrom.Coordinates.System);
            Assert.AreEqual(6, secondEvent.PlanetFrom.Coordinates.Position);
            Assert.AreEqual("Главная планета", secondEvent.PlanetTo.Name);
            Assert.AreEqual(1, secondEvent.PlanetTo.Coordinates.Galaxy);
            Assert.AreEqual(279, secondEvent.PlanetTo.Coordinates.System);
            Assert.AreEqual(10, secondEvent.PlanetTo.Coordinates.Position);
            Assert.IsTrue(secondEvent.IsReturn);
        }

        private string GetTransportTestResponce()
        {
            var testResponceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "FleetEvents.txt");
            var testResponce = File.ReadAllText(testResponceFilePath);
            return testResponce;
        }
    }
}