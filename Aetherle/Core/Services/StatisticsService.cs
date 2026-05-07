using Aetherle.Core.Models;

namespace Aetherle.Core.Services;

public class StatisticsService
{
    public static void UpdateStats(Configuration config, bool won, int attempts)
    {
        config.GamesPlayed++;

        if (won)
        {
            config.GamesWon++;
            config.CurrentStreak++;

            if (config.CurrentStreak > config.MaxStreak)
            {
                config.MaxStreak = config.CurrentStreak;
            }

            if (config.GuessDistribution.TryGetValue(attempts, out int count))
            {
                config.GuessDistribution[attempts] = count + 1;
            }
            else
            {
                config.GuessDistribution[attempts] = 1;
            }
        }
        else
        {
            config.CurrentStreak = 0;
        }

        config.Save();
    }
}
