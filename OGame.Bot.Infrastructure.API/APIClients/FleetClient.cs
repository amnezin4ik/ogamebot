using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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

        public async Task<Fleet> GetFleetAvailableAsync(SessionData sessionData)
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

        public async Task MoveToAttentionPhase(SessionData sessionData, Fleet fleet, Coordinates coordinatesFrom)
        {
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var content = new FormUrlEncodedContent(new Dictionary<string, string>());
                var uriBuilder = new UriBuilder(Constants.OGameUrl);
                var parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters["page"] = "fleet2";
                parameters["galaxy"] = coordinatesFrom.Galaxy.ToString();
                parameters["system"] = coordinatesFrom.System.ToString();
                parameters["position"] = coordinatesFrom.Position.ToString();
                parameters["type"] = "1";
                parameters["mission"] = "0";
                parameters["speed"] = "10";
                foreach (var shipCell in fleet.Ships)
                {
                    var shipTypeInt = (int) shipCell.Ship.ShipType;
                    parameters[$"am{shipTypeInt}"] = shipCell.Count.ToString();
                }
                uriBuilder.Query = parameters.ToString();
                var attentionPhaseUrl = uriBuilder.Uri.AbsoluteUri;
                await _httpHelper.PostAsync(httpClient, attentionPhaseUrl, content);
            }
        }

        public async Task<GoPhaseInfo> MoveToGoPhase(SessionData sessionData, Fleet fleet, Coordinates coordinatesTo, MissionTarget target, MissionType missionType, FleetSpeed fleetSpeed)
        {
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var content = new FormUrlEncodedContent(new Dictionary<string, string>());
                var uriBuilder = new UriBuilder(Constants.OGameUrl);
                var parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters["page"] = "fleet3";
                parameters["galaxy"] = coordinatesTo.Galaxy.ToString();
                parameters["system"] = coordinatesTo.System.ToString();
                parameters["position"] = coordinatesTo.Position.ToString();
                parameters["type"] = $"{(int)target}";
                parameters["mission"] = $"{(int)missionType}";
                parameters["union"] = "0";
                parameters["acsValues"] = "-";
                parameters["speed"] = $"{(int)fleetSpeed}";
                foreach (var shipCell in fleet.Ships)
                {
                    var shipTypeInt = (int)shipCell.Ship.ShipType;
                    parameters[$"am{shipTypeInt}"] = shipCell.Count.ToString();
                }
                uriBuilder.Query = parameters.ToString();
                var attentionPhaseUrl = uriBuilder.Uri.AbsoluteUri;

                var responseMessage = await _httpHelper.PostAsync(httpClient, attentionPhaseUrl, content);
                var contentString = await responseMessage.Content.ReadAsStringAsync();

                var goPhaseInfo = new GoPhaseInfo();
                var document = _htmlParser.Parse(contentString);
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

        public async Task Go(SessionData sessionData, GoPhaseInfo goPhaseInfo, double metal, double crystal, double deuterium)
        {
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var requestContentParameters = new Dictionary<string, string>(goPhaseInfo.AlreadySetedParameters)
                {
                    {"holdingtime", "1"},
                    {"expeditiontime", "1"},
                    {"metal", "200"},
                    {"crystal", "150"},
                    {"deuterium", "100"}
                };
                var content = new FormUrlEncodedContent(requestContentParameters);
                var requestUri = $"{Constants.OGameUrl}?page=movement";
                var responseMessage = await _httpHelper.PostAsync(httpClient, requestUri, content);
            }
        }
    }
}