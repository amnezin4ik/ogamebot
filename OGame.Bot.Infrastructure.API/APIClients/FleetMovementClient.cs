using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using OGame.Bot.Infrastructure.API.Dto;
using OGame.Bot.Infrastructure.API.Helpers;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public class FleetMovementClient : IFleetMovementClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpHelper _httpHelper;
        private readonly HtmlParser _htmlParser;
        private readonly CoordinatesParser _coordinatesParser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FleetMovementClient(IHttpClientFactory httpClientFactory,
            IHttpHelper httpHelper,
            HtmlParser htmlParser,
            CoordinatesParser coordinatesParser,
            IDateTimeProvider dateTimeProvider)
        {
            _httpClientFactory = httpClientFactory;
            _httpHelper = httpHelper;
            _htmlParser = htmlParser;
            _coordinatesParser = coordinatesParser;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<FleetMovement> ReturnFleetAsync(SessionData sessionData, string fleetMovementId)
        {
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                await _httpHelper.GetAsync(httpClient, $"{Constants.OGameUrl}?page=movement&return={fleetMovementId}");
                var allMovements = await GetAllFleetMovementsAsync(sessionData);
                var returnMovement = allMovements.Single(m => m.Id == fleetMovementId);
                return returnMovement;
            }
        }

        public async Task<IEnumerable<FleetMovement>> GetAllFleetMovementsAsync(SessionData sessionData)
        {
            var fleetEvents = await GetFleetMovementsByTypeAsync(sessionData, null);
            return fleetEvents;
        }

        public async Task<IEnumerable<FleetMovement>> GetFleetMovementsAsync(SessionData sessionData, MissionType missionType)
        {
            var fleetEvents = await GetFleetMovementsByTypeAsync(sessionData, missionType);
            return fleetEvents;
        }

        private async Task<IEnumerable<FleetMovement>> GetFleetMovementsByTypeAsync(SessionData sessionData, MissionType? missionType)
        {
            var fleetMovements = new List<FleetMovement>();
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var movementContent = await _httpHelper.GetStringAsync(httpClient, $"{Constants.OGameUrl}?page=movement");
                var document = _htmlParser.Parse(movementContent);
                var fleetMovementElements = document.QuerySelectorAll("div.fleetDetails").ToList();
                if (fleetMovementElements.Any())
                {
                    foreach (var fleetMovementElement in fleetMovementElements)
                    {
                        var missionTypeInt = int.Parse(fleetMovementElement.GetAttribute("data-mission-type"));
                        var currentMissionType = (MissionType)missionTypeInt;
                        if (missionType.HasValue && currentMissionType != missionType)
                        {
                            continue;
                        }
                        var movementId = fleetMovementElement.GetAttribute("id").Replace("fleet", string.Empty);
                        var arrivalTimeSeconds = double.Parse(fleetMovementElement.GetAttribute("data-arrival-time"));
                        var arrivalTimeUtc = _dateTimeProvider.GetOGameUtcOffset() + TimeSpan.FromSeconds(arrivalTimeSeconds);

                        var originPlanetName = fleetMovementElement.QuerySelector("span.originData > span.originPlanet").TextContent.Trim();
                        var originCoordsString = fleetMovementElement.QuerySelector("span.originData > span.originCoords").TextContent.Trim();
                        var originCoordinates = _coordinatesParser.ParseCoordinatesFromString(originCoordsString);
                        var planetFrom = new MissionPlanet { Name = originPlanetName, Coordinates = originCoordinates };

                        var destPlanetName = fleetMovementElement.QuerySelector("span.destinationData > span.destinationPlanet").TextContent.Trim();
                        var destCoordsString = fleetMovementElement.QuerySelector("span.destinationData > span.destinationCoords").TextContent.Trim();
                        var destCoordinates = _coordinatesParser.ParseCoordinatesFromString(destCoordsString);
                        var planetTo = new MissionPlanet { Name = destPlanetName, Coordinates = destCoordinates };

                        var isReturn = bool.Parse(fleetMovementElement.GetAttribute("data-return-flight"));

                        var mission = new FleetMovement
                        {
                            Id = movementId,
                            MissionType = currentMissionType,
                            ArrivalTimeUtc = arrivalTimeUtc,
                            PlanetFrom = planetFrom,
                            PlanetTo = planetTo,
                            IsReturn = isReturn
                        };

                        if (!isReturn)
                        {
                            var arrivalTimeString = fleetMovementElement.QuerySelector("span.timer").Attributes["title"].Value.Trim();
                            var arrivalDateTime = DateTime.ParseExact(arrivalTimeString, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            var returnTimeString = fleetMovementElement.QuerySelector("span.nextTimer").Attributes["title"].Value.Trim();
                            var returnDateTime = DateTime.ParseExact(returnTimeString, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            var oneWayFlightTime = returnDateTime - arrivalDateTime;
                            var returnTimeUtc = arrivalTimeUtc + oneWayFlightTime;
                            mission.ReturnTimeUtc = returnTimeUtc;
                        }

                        fleetMovements.Add(mission);
                    }
                }
            }
            return fleetMovements;
        }
    }
}