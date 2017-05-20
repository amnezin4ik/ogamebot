using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using OGame.Bot.Infrastructure.API.Dto;
using OGame.Bot.Infrastructure.API.Helpers;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public class MissionClient : IMissionClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpHelper _httpHelper;
        private readonly HtmlParser _htmlParser;

        public MissionClient(IHttpClientFactory httpClientFactory, IHttpHelper httpHelper, HtmlParser htmlParser)
        {
            _httpClientFactory = httpClientFactory;
            _httpHelper = httpHelper;
            _htmlParser = htmlParser;
        }

        public async Task<IEnumerable<Mission>> GetAllMissionsAsync(SessionData sessionData)
        {
            var fleetEvents = new List<Mission>();
            var handler = new HttpClientHandler { CookieContainer = sessionData.RequestCookies };
            using (var httpClient = _httpClientFactory.GetHttpClient(handler))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var overviewContent = await _httpHelper.GetStringAsync(httpClient, "https://s140-ru.ogame.gameforge.com/game/index.php?page=eventList");
                var document = _htmlParser.Parse(overviewContent);
                var fleetEventElements = document.QuerySelectorAll("tr[class=eventFleet]").ToList();
                if (fleetEventElements.Any())
                {
                    foreach (var fleetEventElement in fleetEventElements)
                    {
                        var eventId = fleetEventElement.GetAttribute("id");
                        var missionType = int.Parse(fleetEventElement.GetAttribute("data-mission-type"));
                        var arrivalTimeSeconds = double.Parse(fleetEventElement.GetAttribute("data-arrival-time"));

                        var originPlanetName = fleetEventElement.QuerySelector("td[class=originFleet]").TextContent.Trim();
                        var originCoordsString = fleetEventElement.QuerySelector("td[class=coordsOrigin]").TextContent.Trim();
                        var originCoordinates = ParseCoordinatesFromString(originCoordsString);
                        var planetFrom = new MissionPlanet { PlanetName = originPlanetName , PlanetCoordinates = originCoordinates };

                        var destPlanetName = fleetEventElement.QuerySelector("td[class=destFleet]").TextContent.Trim();
                        var destCoordsString = fleetEventElement.QuerySelector("td[class=destCoords]").TextContent.Trim();
                        var destCoordinates = ParseCoordinatesFromString(destCoordsString);
                        var planetTo = new MissionPlanet { PlanetName = destPlanetName, PlanetCoordinates = destCoordinates };

                        var fleetEvent = new Mission
                        {
                            Id = eventId,
                            MissionType = (MissionType)missionType,
                            ArrivalTimeUtc = TimeSpan.FromSeconds(arrivalTimeSeconds),
                            PlanetFrom = planetFrom,
                            PlanetTo = planetTo
                        };
                        fleetEvents.Add(fleetEvent);
                    }
                }
                else
                {
                    //TODO: chech eventsList container, if not exists - send error code
                }
            }
            return fleetEvents;
        }

        private Coordinates ParseCoordinatesFromString(string coordinatesString)
        {
            Coordinates coordinates = null;
            if (!string.IsNullOrWhiteSpace(coordinatesString))
            {
                var coordinatesArray = coordinatesString.Trim().TrimStart('[').TrimEnd(']').Split(':');
                if (coordinatesArray.Length == 3)
                {
                    int galaxy, system, position;
                    if (int.TryParse(coordinatesArray[0], out galaxy) &&
                        int.TryParse(coordinatesArray[1], out system) &&
                        int.TryParse(coordinatesArray[2], out position))
                    {
                        coordinates = new Coordinates
                        {
                            Galaxy = galaxy,
                            System = system,
                            Position = position
                        };
                    }
                }
            }
            return coordinates;
        }
    }
}
