using ExpressionTree;
using Sunrise.Server.Data;
using Sunrise.Server.Objects.Models;
using Sunrise.Server.Types.Enums;

namespace Sunrise.Server.Utils;

public static class Configuration
{
    public static bool IgnoreBeatmapRanking => true;
    public static string RedisConnection => "localhost:6379";
    public static string WelcomeMessage => "Welcome to Sunrise!";
    public static string BotUsername => "Sunshine Bot";
    public static string BotPrefix => "!";
    public static string Domain => "sunrise.local";
    public static bool OnMaintenance { get; set; } = false;
    public static int UserApiCallsInMinute => 50;
    
 public static void InsertApiServersIfNotExists()
    {
        var apis = new List<ExternalApi>
        {
            new ExternalApi().Fill(ApiType.BeatmapDownload, ApiServer.OldPpy, "https://old.ppy.sh/osu/{0}", 0, 1),
            new ExternalApi().Fill(ApiType.BeatmapDownload, ApiServer.CatboyBest, "https://catboy.best/osu/{0}n", 1, 1),
            new ExternalApi().Fill(ApiType.BeatmapDownload, ApiServer.OsuDirect, "https://osu.direct/api/osu/{0}", 2, 1),
            new ExternalApi().Fill(ApiType.BeatmapSetDataById, ApiServer.OsuDirect, "https://osu.direct/api/v2/s/{0}", 0, 1),
            new ExternalApi().Fill(ApiType.BeatmapSetDataByBeatmapId, ApiServer.OsuDirect, "https://osu.direct/api/v2/b/{0}?full=true", 0, 1),
            new ExternalApi().Fill(ApiType.BeatmapSetDataByHash, ApiServer.OsuDirect, "https://osu.direct/api/v2/md5/{0}?full=true", 0, 1)
        };

        // TODO: Add more mirrors (need also more serializers?)
        
        var database = ServicesProviderHolder.ServiceProvider.GetRequiredService<SunriseDb>().GetOrm();
        
        if (database == null)
        {
            throw new Exception("Don't try to call this method before the database is initialized.");
        }
        
        foreach (var api in from api in apis let existingApi = database.SelectFirst<ExternalApi>(new Expr("Url", OperatorEnum.Equals, api.Url)) where existingApi == null select api)
        {
            database.Insert(api);
        }
    }
}