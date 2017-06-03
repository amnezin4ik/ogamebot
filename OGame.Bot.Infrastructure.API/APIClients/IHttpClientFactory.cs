using System.Net.Http;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient(SessionData sessionData);

        HttpClient GetHttpClient(HttpClientHandler httpClientHandler);
    }
}