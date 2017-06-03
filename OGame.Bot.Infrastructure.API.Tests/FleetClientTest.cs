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
            await fleetClient.MoveToAttentionPhase(sessionData, availableFleet, coordinatesFrom);

            var coordinatesTo = new Coordinates
            {
                Galaxy = 1,
                System = 299,
                Position = 9
            };
            var moveToGoPhaseInfo = await fleetClient.MoveToGoPhase(sessionData, availableFleet, coordinatesTo, MissionTarget.Planet, MissionType.Leave, FleetSpeed.Percent10);

            double metal = 300;
            double crystal=500;
            double deuterium=1000;

            await fleetClient.Go(sessionData, moveToGoPhaseInfo, metal, crystal, deuterium);
        }
    }
}
