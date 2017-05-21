using System;

namespace OGame.Bot.Modules.Common
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public TimeSpan GetUtcNow()
        {
            return new TimeSpan(DateTime.UtcNow.Ticks);
        }
    }
}