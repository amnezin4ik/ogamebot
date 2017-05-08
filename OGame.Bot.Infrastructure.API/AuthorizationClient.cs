using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Models;

namespace OGame.Bot.Infrastructure.API
{
    //assol2009.89@mail.ru
    //Ваше игровое имя: user7395496
    //Старый пароль: 2016895 
    //Сервер: Nusakan

    public class AuthorizationClient : IAuthorizationClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthorizationClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<SessionData> LogInAsync(string userName = "user7395496", string password = "2016895")
        {
            var values = new Dictionary<string, string>
            {
               { "kid", "" },
               { "login", userName },
               { "pass", password },
               { "uni", "s140-ru.ogame.gameforge.com" }
            };

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            };

            using (var httpClient = _httpClientFactory.GetHttpClient(handler))
            {
                var content = new FormUrlEncodedContent(values);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                await httpClient.PostAsync("https://ru.ogame.gameforge.com/main/login", content);
                var result = new SessionData(cookieContainer);
                return result;
            }
        }
    }
}
