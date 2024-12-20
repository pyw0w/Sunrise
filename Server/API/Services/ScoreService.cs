using osu.Shared;
using Sunrise.Server.Application;
using Sunrise.Server.Database;
using Sunrise.Server.Database.Models;

namespace Sunrise.Server.API.Services;

public static class ScoreService
{
    public static async Task<Score?> GetScore(int id)
    {
        var database = ServicesProviderHolder.GetRequiredService<DatabaseManager>();
        return await database.ScoreService.GetScore(id);
    }

    public static async Task<List<Score>> GetScoresByUser(int userId, GameMode mode)
    {
        var database = ServicesProviderHolder.GetRequiredService<DatabaseManager>();
        return await database.ScoreService.GetUserBestScores(userId, mode);
    }
}