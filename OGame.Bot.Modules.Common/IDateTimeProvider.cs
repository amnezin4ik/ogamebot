using System;

namespace OGame.Bot.Modules.Common
{
    public interface IDateTimeProvider
    {
        TimeSpan GetUtcNow();
    }
}