using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using OGame.Bot.Infrastructure.API.Dto;
using OGame.Bot.Infrastructure.API.Helpers;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public class PlanetOverviewClient : IPlanetOverviewClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpHelper _httpHelper;
        private readonly HtmlParser _htmlParser;

        public PlanetOverviewClient(IHttpClientFactory httpClientFactory, IHttpHelper httpHelper, HtmlParser htmlParser)
        {
            _httpClientFactory = httpClientFactory;
            _httpHelper = httpHelper;
            _htmlParser = htmlParser;
        }

        public async Task<PlanetOverview> GetPlanetOverviewAsync(SessionData sessionData, UserPlanet userPlanet)
        {
            var planetOverview = new PlanetOverview
            {
                UserPlanet = userPlanet,
                Resources = new Resources()
            };
            using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            {
                httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");
                var contentString = await _httpHelper.GetStringAsync(httpClient, $"{Constants.OGameUrl}?page=overview&cp={userPlanet.PlanetId}");
                var document = _htmlParser.Parse(contentString);

                var resourceElements = document.QuerySelectorAll("ul[id=resources] > li > span > span").ToList();
                foreach (var resourceElement in resourceElements)
                {
                    var resourceCountString = resourceElement.InnerHtml.Replace(".", string.Empty).Trim();
                    var resourceCount = int.Parse(resourceCountString);
                    switch (resourceElement.Id)
                    {
                        case "resources_metal":
                            planetOverview.Resources.Metal = resourceCount;
                            break;
                        case "resources_crystal":
                            planetOverview.Resources.Crystal = resourceCount;
                            break;
                        case "resources_deuterium":
                            planetOverview.Resources.Deuterium = resourceCount;
                            break;
                        case "resources_energy":
                            planetOverview.Energy = resourceCount;
                            break;
                    }
                }
            }
            return planetOverview;
        }
    }
}