using System.Security.Claims;
using CleverDocs.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CleverDocs.Infrastructure.Services;

public class UserContext(
    IHttpContextAccessor httpContextAccessor,
    AppDbContext dbContext,
    IMemoryCache memoryCache)
{
    private const string CacheKeyPrefix = "users:id:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public async Task<string?> GetUserIdAsync(CancellationToken cancellationToken = default)
    {
        string? identityId = httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (identityId is null)
        {
            return null;
        }

        string cacheKey = $"{CacheKeyPrefix}{identityId}";

        string? userId = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(CacheDuration);

            string? userId = await dbContext.Users
                .Where(u => u.IdentityId == identityId)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return userId;
        });

        return userId;
    }
}