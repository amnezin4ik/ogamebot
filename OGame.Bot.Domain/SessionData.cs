using System.Net;

namespace OGame.Bot.Domain
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
