using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly CoordinatesParser _coordinatesParser;

        public MissionClient(IHttpClientFactory httpClientFactory, IHttpHelper httpHelper, HtmlParser htmlParser, CoordinatesParser coordinatesParser)
        {
            _httpClientFactory = httpClientFactory;
            _httpHelper = httpHelper;
            _htmlParser = htmlParser;
            _coordinatesParser = coordinatesParser;
        }

        public async Task<IEnumerable<Mission>> GetAllMissionsAsync(SessionData sessionData)
        {
            var fleetEvents = await GetMissionsByTypeAsync(sessionData, null);
            return fleetEvents;
        }

        public async Task<IEnumerable<Mission>> GetMissionsAsync(SessionData sessionData, MissionType missionType)
        {
            var fleetEvents = await GetMissionsByTypeAsync(sessionData, missionType);
            return fleetEvents;
        }

        private async Task<IEnumerable<Mission>> GetMissionsByTypeAsync(SessionData sessionData, MissionType? missionType)
        {
            var fleetEvents = new List<Mission>();
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var overviewContent = await _httpHelper.GetStringAsync(httpClient, $"{Constants.OGameUrl}?page=eventList");
                var document = _htmlParser.Parse(overviewContent);
                var fleetEventElements = document.QuerySelectorAll("tr[class=eventFleet]").ToList();
                if (fleetEventElements.Any())
                {
                    foreach (var fleetEventElement in fleetEventElements)
                    {
                        var missionTypeInt = int.Parse(fleetEventElement.GetAttribute("data-mission-type"));
                        var currentMissionType = (MissionType) missionTypeInt;
                        if (missionType.HasValue && currentMissionType != missionType)
                        {
                            continue;
                        }
                        var eventId = fleetEventElement.GetAttribute("id");
                        var arrivalTimeSeconds = double.Parse(fleetEventElement.GetAttribute("data-arrival-time"));

                        var originPlanetName = fleetEventElement.QuerySelector("td[class=originFleet]").TextContent.Trim();
                        var originCoordsString = fleetEventElement.QuerySelector("td[class=coordsOrigin]").TextContent.Trim();
                        var originCoordinates = _coordinatesParser.ParseCoordinatesFromString(originCoordsString);
                        var planetFrom = new MissionPlanet { PlanetName = originPlanetName, PlanetCoordinates = originCoordinates };

                        var destPlanetName = fleetEventElement.QuerySelector("td[class=destFleet]").TextContent.Trim();
                        var destCoordsString = fleetEventElement.QuerySelector("td[class=destCoords]").TextContent.Trim();
                        var destCoordinates = _coordinatesParser.ParseCoordinatesFromString(destCoordsString);
                        var planetTo = new MissionPlanet { PlanetName = destPlanetName, PlanetCoordinates = destCoordinates };

                        var fleetEvent = new Mission
                        {
                            Id = eventId,
                            MissionType = currentMissionType,
                            ArrivalTimeUtc = TimeSpan.FromSeconds(arrivalTimeSeconds),
                            PlanetFrom = planetFrom,
                            PlanetTo = planetTo
                        };
                        fleetEvents.Add(fleetEvent);
                    }
                }
            }
            return fleetEvents;
        }

        
    }
}
