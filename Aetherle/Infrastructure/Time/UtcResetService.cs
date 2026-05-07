using System;

namespace Aetherle.Infrastructure.Time;

public class UtcResetService
{
    public static void CheckAndResetDailyLimits(Configuration config)
    {
        if (DateTime.UtcNow.Date > config.LastPlayDate.Date)
        {
            config.DailyGamesCompletions = 0;
            config.LastPlayDate = DateTime.UtcNow.Date;
            Plugin.PluginInterface.SavePluginConfig(config);
        }
    }
}
