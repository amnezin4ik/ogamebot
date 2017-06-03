using System.Net.Http;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient GetHttpClient(SessionData sessionData)
        {
            var httpClientHandler = new HttpClientHandler
            {
                CookieContainer = sessionData.RequestCookies
            };
            var httpClient = new HttpClient(httpClientHandler);
            SetDefaultHeaders(httpClient);
            return httpClient;
        }

        public HttpClient GetHttpClient(HttpClientHandler httpClientHandler)
        {
            var httpClient = new HttpClient(httpClientHandler);
            SetDefaultHeaders(httpClient);
            return httpClient;
        }

        private void SetDefaultHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0");
            httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }
    }
}