using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using OGame.Bot.Infrastructure.API.Dto;
using OGame.Bot.Infrastructure.API.Helpers;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public class UserPlanetsClient : IUserPlanetsClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpHelper _httpHelper;
        private readonly HtmlParser _htmlParser;
        private readonly CoordinatesParser _coordinatesParser;

        public UserPlanetsClient(IHttpClientFactory httpClientFactory, IHttpHelper httpHelper, HtmlParser htmlParser, CoordinatesParser coordinatesParser)
        {
            _httpClientFactory = httpClientFactory;
            _httpHelper = httpHelper;
            _htmlParser = htmlParser;
            _coordinatesParser = coordinatesParser;
        }

        public async Task<IEnumerable<UserPlanet>> GetUserPlanetsAsync(SessionData sessionData)
        {
            var userPlanets = new List<UserPlanet>();
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var contentString = await _httpHelper.GetStringAsync(httpClient, $"{Constants.OGameUrl}?page=overview");
                var document = _htmlParser.Parse(contentString);
                var planetElements = document.QuerySelectorAll("div[id=planetList] > div.smallplanet").ToList();
                foreach (var planetElement in planetElements)
                {
                    var planetId = planetElement.Attributes["id"].Value.Replace("planet-", "").Trim();
                    var planetName = planetElement.QuerySelector("span.planet-name").InnerHtml.Trim();
                    var planetCoordinatesString = planetElement.QuerySelector("span.planet-koords").InnerHtml.Trim();
                    var planetCoordinates = _coordinatesParser.ParseCoordinatesFromString(planetCoordinatesString);
                    var userPlanet = new UserPlanet
                    {
                        PlanetId = planetId,
                        Name = planetName,
                        Coordinates = planetCoordinates
                    };
                    userPlanets.Add(userPlanet);
                }
            }
            return userPlanets;
        }

        public async Task SetActivePlanetAsync(SessionData sessionData, string planetId)
        {
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                await _httpHelper.GetAsync(httpClient, $"{Constants.OGameUrl}?page=overview&cp={planetId}");
            }
        }
    }
}