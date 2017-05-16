using System.Net;

namespace OGame.Bot.Infrastructure.API.Dto
{
    public sealed class SessionData
    {
        public SessionData(CookieContainer requestCookies)
        {
            RequestCookies = requestCookies;
        }

        public CookieContainer RequestCookies { get; }
    }
}
