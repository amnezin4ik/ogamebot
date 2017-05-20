using System;

namespace OGame.Bot.Modules.Common
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
