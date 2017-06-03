﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using OGame.Bot.Infrastructure.API.Helpers;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public class ResourceBuildingsClient : IResourceBuildingsClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpHelper _httpHelper;

        public ResourceBuildingsClient(IHttpClientFactory httpClientFactory, IHttpHelper httpHelper)
        {
            _httpClientFactory = httpClientFactory;
            _httpHelper = httpHelper;
        }

        //TODO: Build Resource
        public async Task GetResourceBuildingsAsync(SessionData sessionData, ResourceBuildingType resourceBuildingType)
        {
            //using (var httpClient = _httpClientFactory.GetHttpClient(sessionData))
            //{
            //    httpClient.DefaultRequestHeaders.Add("Host", "s140-ru.ogame.gameforge.com");

            //    var overviewContent = await _httpHelper.GetStringAsync(httpClient, $"{Constants.OGameUrl}?page=resources");

            //    var parser = new HtmlParser();
            //    var document = parser.Parse(overviewContent);
            //    var buildLinks = document.QuerySelectorAll("a")
            //        .Where(element => element.Attributes
            //            .Any(attr => attr.Value.Contains($"page=resources&modus=1&type={(int)resourceBuildingType}&menge=1&token=")))
            //        .ToList();

            //    var token = string.Empty;

            //    if (buildLinks.Any())
            //    {
            //        //TODO: Do we need parse token or we could get full URL?
            //        var buttonOnClickAttribute = buildLinks.First().Attributes.First(attr => attr.Name == "onclick");
            //        var regex = new Regex("token=[a-z0-9]{32}'");
            //        var tokenMatch = regex.Match(buttonOnClickAttribute.Value);
            //        if (!string.IsNullOrEmpty(tokenMatch.Value))
            //        {
            //            token = tokenMatch.Value.Substring(6, 32);
            //        }
            //    }

            //    var content = new FormUrlEncodedContent(new Dictionary<string, string>());

            //    var url = $"{Constants.OGameUrl}?page=resources&modus=1&type={(int)resourceBuildingType}&menge=1&token={token}";
            //    var response = await _httpHelper.PostAsync(httpClient, url, content);
            //}
        }
    }
}