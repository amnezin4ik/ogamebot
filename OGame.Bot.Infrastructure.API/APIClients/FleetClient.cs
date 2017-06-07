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
    public class FleetClient : IFleetClient
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

        public async Task<Fleet> GetActivePlanetFleetAsync(SessionData sessionData)
        {
            var shipCells = new List<ShipCell>();
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var contentString = await _httpHelper.GetStringAsync(httpClient, $"{Constants.OGameUrl}?page=fleet1");
                var document = _htmlParser.Parse(contentString);
                var availableShipCells = document.QuerySelectorAll("ul[id=military] > li.on, ul[id=civil] > li.on").ToList();
                foreach (var availableShipCell in availableShipCells)
                {
                    var shipTypeString = availableShipCell.Attributes["id"].Value.Replace("button", string.Empty).Trim();
                    var shipType = (ShipType)Enum.Parse(typeof(ShipType), shipTypeString);
                    var shipInfo = availableShipCell.QuerySelector("span.level");
                    var shipInfoString = shipInfo.FirstElementChild.OuterHtml;
                    var shipCountString = shipInfo.InnerHtml.Replace(shipInfoString, string.Empty).Trim();
                    var shipCount = int.Parse(shipCountString);
                    var shipCell = new ShipCell
                    {
                        Ship = new Ship { ShipType = shipType },
                        Count = shipCount
                    };
                    shipCells.Add(shipCell);
                }
            }
            var fleet = new Fleet
            {
                Ships = shipCells
            };
            return fleet;
        }

        public async Task SendFleetPhase1(SessionData sessionData, Fleet fleet, Coordinates coordinatesFrom)
        {
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var contentParams = new Dictionary<string, string>
                {
                    ["page"] = "fleet2",
                    ["galaxy"] = coordinatesFrom.Galaxy.ToString(),
                    ["system"] = coordinatesFrom.System.ToString(),
                    ["position"] = coordinatesFrom.Position.ToString(),
                    ["type"] = "1",
                    ["mission"] = "0",
                    ["speed"] = "10"
                };
                foreach (var shipCell in fleet.Ships)
                {
                    var shipTypeInt = (int)shipCell.Ship.ShipType;
                    contentParams[$"am{shipTypeInt}"] = shipCell.Count.ToString();
                }
                var content = new FormUrlEncodedContent(contentParams);
                var requestUri = $"{Constants.OGameUrl}?page=fleet2";
                await _httpHelper.PostAsync(httpClient, requestUri, content);
            }
        }

        public async Task<SendFleetPhase3Info> SendFleetPhase2(SessionData sessionData, Fleet fleet, Coordinates coordinatesTo, MissionTarget target, MissionType missionType, FleetSpeed fleetSpeed)
        {
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var contentParams = new Dictionary<string, string>
                {
                    ["galaxy"] = coordinatesTo.Galaxy.ToString(),
                    ["system"] = coordinatesTo.System.ToString(),
                    ["position"] = coordinatesTo.Position.ToString(),
                    ["type"] = $"{(int) target}",
                    ["mission"] = $"{(int) missionType}",
                    ["union"] = "0",
                    ["acsValues"] = "-",
                    ["speed"] = $"{(int) fleetSpeed}"
                };
                foreach (var shipCell in fleet.Ships)
                {
                    var shipTypeInt = (int)shipCell.Ship.ShipType;
                    contentParams[$"am{shipTypeInt}"] = shipCell.Count.ToString();
                }
                var content = new FormUrlEncodedContent(contentParams);
                var requestUri = $"{Constants.OGameUrl}?page=fleet3";
                var responseMessage = await _httpHelper.PostAsync(httpClient, requestUri, content);
                var responceString = await responseMessage.Content.ReadAsStringAsync();
                var goPhaseInfo = GetPhase3Info(responceString);
                return goPhaseInfo;
            }
        }

        public async Task SendFleetPhase3(SessionData sessionData, SendFleetPhase3Info sendFleetPhase3Info, Resources resources)
        {
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var contentParams = new Dictionary<string, string>(sendFleetPhase3Info.AlreadySetedParameters)
                {
                    {"holdingtime", "1"},
                    {"expeditiontime", "1"},
                    {"metal", resources.Metal.ToString()},
                    {"crystal", resources.Crystal.ToString()},
                    {"deuterium", resources.Deuterium.ToString()}
                };
                var content = new FormUrlEncodedContent(contentParams);
                var requestUri = $"{Constants.OGameUrl}?page=movement";
                await _httpHelper.PostAsync(httpClient, requestUri, content);
            }
        }

        private SendFleetPhase3Info GetPhase3Info(string responceString)
        {
            var goPhaseInfo = new SendFleetPhase3Info();
            var document = _htmlParser.Parse(responceString);
            var hiddenFields = document.QuerySelectorAll("input[type=hidden]").ToList();
            foreach (var hiddenField in hiddenFields)
            {
                var name = hiddenField.Attributes["name"].Value;
                var value = hiddenField.Attributes["value"].Value;
                goPhaseInfo.AlreadySetedParameters.Add(name, value);
            }
            var barContainerDiv = document.QuerySelector("div.bar_container");
            var fleetLoadRoomCapacityString = barContainerDiv.Attributes["data-capacity"].Value;
            goPhaseInfo.FleetLoadRoomCapacity = double.Parse(fleetLoadRoomCapacityString);
            return goPhaseInfo;
        }
    }
}