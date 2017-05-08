using System.Net;

namespace OGame.Bot.Infrastructure.API.Models
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
