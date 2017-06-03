using System.Net.Http;
using System.Threading.Tasks;

namespace OGame.Bot.Infrastructure.API.Helpers
{
    public interface IHttpHelper
    {
        Task<HttpResponseMessage> GetAsync(HttpClient httpClient, string requestUri);

        Task<string> GetStringAsync(HttpClient httpClient, string requestUri);

        Task<HttpResponseMessage> PostAsync(HttpClient httpClient, string requestUri, HttpContent content);
    }
}