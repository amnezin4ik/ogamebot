using System;

namespace OGame.Bot.Modules.Common
{
    public interface IDateTimeProvider
    {
        DateTime GetUtcNow();
    }
}