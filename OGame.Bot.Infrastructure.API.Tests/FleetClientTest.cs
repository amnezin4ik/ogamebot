using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using NUnit.Framework;
using OGame.Bot.Infrastructure.API.APIClients;
using OGame.Bot.Infrastructure.API.Dto;
using OGame.Bot.Infrastructure.API.Helpers;

namespace OGame.Bot.Infrastructure.API.Tests
{
    [TestFixture]
    public class FleetClientTest
    {
        [Test]
        public async Task SendFleet()
        {
            var httpClientFactory = new HttpClientFactory();
            var authorizationClient = new AuthorizationClient(httpClientFactory);
            var httpHelper = new HttpHelper();
            var htmlParser = new HtmlParser();
            var fleetClient= new FleetClient(httpClientFactory, httpHelper, htmlParser);

            var sessionData = await authorizationClient.LogInAsync();

            var availableFleet = await fleetClient.GetFleetAvailableAsync(sessionData);


            var coordinatesFrom = new Coordinates
            {
                Galaxy = 1,
                System = 279,
                Position = 6
            };
            await fleetClient.SendFleetPhase1(sessionData, availableFleet, coordinatesFrom);

            var coordinatesTo = new Coordinates
            {
                Galaxy = 1,
                System = 299,
                Position = 9
            };
            var moveToGoPhaseInfo = await fleetClient.SendFleetPhase2(sessionData, availableFleet, coordinatesTo, MissionTarget.Planet, MissionType.Leave, FleetSpeed.Percent10);

            var metal = 300;
            var crystal=500;
            var deuterium=1000;

            await fleetClient.SendFleetPhase3(sessionData, moveToGoPhaseInfo, metal, crystal, deuterium);
        }
    }
}