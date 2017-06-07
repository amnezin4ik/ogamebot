using System;
using System.IO;
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
    public class PlanetOverviewClientTest
    {
        [Test]
        public async Task ShouldReturnPlanetOverview()
        {
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(m => m.GetHttpClient(It.IsAny<SessionData>()))
                .Returns(new HttpClient());

            var testResponce = GetPlanetOverviewTestResponce();
            var httpHelperMock = new Mock<IHttpHelper>();
            httpHelperMock
                .Setup(m => m.GetStringAsync(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .ReturnsAsync(testResponce);

            var htmlParser = new HtmlParser();
            var planetOverviewClient = new PlanetOverviewClient(httpClientFactoryMock.Object, httpHelperMock.Object, htmlParser);
            var cookieContainer = new CookieContainer();
            var sessionData = new SessionData(cookieContainer);
            var userPlanet = new UserPlanet();


            var planetOverview = await planetOverviewClient.GetPlanetOverviewAsync(sessionData, userPlanet);


            Assert.AreEqual(userPlanet, planetOverview.UserPlanet);
            Assert.AreEqual(267585, planetOverview.Resources.Metal);
            Assert.AreEqual(36586, planetOverview.Resources.Crystal);
            Assert.AreEqual(40140, planetOverview.Resources.Deuterium);
            Assert.AreEqual(121, planetOverview.Energy);
        }

        private string GetPlanetOverviewTestResponce()
        {
            var testResponceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "PlanetOverview.txt");
            var testResponce = File.ReadAllText(testResponceFilePath);
            return testResponce;
        }
    }
}