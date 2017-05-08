using System.Net.Http;

namespace OGame.Bot.Infrastructure.API
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient();

        HttpClient GetHttpClient(HttpClientHandler httpClientHandler);
    }
}