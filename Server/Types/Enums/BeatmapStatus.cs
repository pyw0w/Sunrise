﻿namespace Sunrise.Server.Types.Enums;

public enum BeatmapStatus
{
    NotSubmitted = -1,
    Pending = 0,
    NeedsUpdate = 1,
    Ranked = 2,
    Approved = 3,
    Qualified = 4,
    Loved = 5
}