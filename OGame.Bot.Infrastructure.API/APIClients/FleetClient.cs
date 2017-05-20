using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using OGame.Bot.Infrastructure.API.Dto;
using OGame.Bot.Infrastructure.API.Helpers;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public class FleetClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpHelper _httpHelper;
        private readonly HtmlParser _htmlParser;

        public FleetClient(IHttpClientFactory httpClientFactory, IHttpHelper httpHelper, HtmlParser htmlParser)
        {
            _httpClientFactory = httpClientFactory;
            _httpHelper = httpHelper;
            _htmlParser = htmlParser;
        }

        public async Task<Fleet> GetFleetAvailableAsync(SessionData sessionData)
        {
            var fleet = new Fleet();
            var handler = new HttpClientHandler { CookieContainer = sessionData.RequestCookies };
            using (var httpClient = _httpClientFactory.GetHttpClient(handler))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var overviewContent = await _httpHelper.GetStringAsync(httpClient, "https://s140-ru.ogame.gameforge.com/game/index.php?page=shipyard");
                var document = _htmlParser.Parse(overviewContent);
                var availableShipCells = document.QuerySelectorAll("ul[id=military] > li.on, ul[id=civil] > li.on").ToList();
                foreach (var shipCell in availableShipCells)
                {
                    var shipType = shipCell.Attributes["id"].Value.Replace("button", string.Empty).Trim();
                    var count = shipCell.QuerySelector("span.level");
                }
            }

            return fleet;
        }

        public void MoveToAttentionPhase(Fleet fleet)
        {
            
        }

        public void MoveToGoPhase(Coordinates missionCoordinates, MissionTarget target, FleetSpeed speed)
        {
            
        }

        public void Go(MissionType missionType, /*Resources*/ bool takeAllResources = false)
        {

        }
    }
}