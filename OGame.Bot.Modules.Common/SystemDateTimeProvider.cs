using System;

namespace OGame.Bot.Modules.Common
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public TimeSpan GetUtcNow()
        {
            return new TimeSpan(DateTime.UtcNow.Ticks);
        }

        public DateTime GetUtcDateNow()
        {
            return DateTime.UtcNow;
        }

        public TimeSpan GetOGameUtcOffset()
        {
            var oGameStartDate = new DateTime(1970, 1, 1);
            return new TimeSpan(oGameStartDate.Ticks);
        }
    }
}