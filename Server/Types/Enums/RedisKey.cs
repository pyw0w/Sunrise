namespace Sunrise.Server.Types.Enums;

public static class RedisKey
{
    public const string OsuVersion = "osu:version:{0}";

    public const string User = "user:{0}";
    public const string UserStats = "user:{0}:stats:{1}";
    public const string Score = "score:{0}";
    public const string Scores = "scores:{0}:leaderboardtype:{1}";
    public const string BeatmapHash = "beatmap:hash:{0}";
    public const string Beatmap = "beatmap:{0}";
    public const string BeatmapSet = "beatmapset:{0}";
    public const string BeatmapSetBySet = "beatmapset:byset:{0}"; // lol nice naming
    public const string BeatmapSearch = "beatmapset:serach:{0}";
    public const string BeatmapFile = "beatmap:{0}:file";

    public const string Avatar = "avatar:{0}";
    public const string Banner = "banner:{0}";
    public const string Replay = "replay:{0}";

    public const string LeaderboardGlobal = "leaderboard:global:{0}";
}