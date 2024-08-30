﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Sunrise.Server.API.Managers;
using Sunrise.Server.API.Serializable.Response;
using Sunrise.Server.Attributes;
using Sunrise.Server.Helpers;
using RateLimiter = System.Threading.RateLimiting.RateLimiter;

namespace Sunrise.Server.API.Controllers;

[ApiController]
[Subdomain("api")]
public class BaseController(IMemoryCache cache) : ControllerBase
{
    [HttpGet]
    [Route("/ping")]
    public IActionResult Index()
    {
        return Ok("Sunrise API");
    }

    [HttpGet]
    [Route("/limits")]
    public async Task<IActionResult> GetLimits()
    {
        var key = RegionHelper.GetUserIpAddress(Request);
        var limiter = cache.Get(key) as RateLimiter;
        var statistics = limiter?.GetStatistics();

        var session = await Request.GetSessionFromRequest();

        return Ok(new LimitsResponse(statistics?.CurrentAvailablePermits, session?.GetRemainingCalls()));
    }
}