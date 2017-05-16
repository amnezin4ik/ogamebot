using System.Net.Http;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient();

        HttpClient GetHttpClient(HttpClientHandler httpClientHandler);
    }
}