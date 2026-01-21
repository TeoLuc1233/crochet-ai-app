using StackExchange.Redis;
using System.Text.Json;

namespace CrochetAI.Api.Services;

public class RedisService : IRedisService, IDisposable
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = redis.GetDatabase();
    }

    public Task<bool> IsConnectedAsync()
    {
        return Task.FromResult(_redis.IsConnected);
    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        await _database.StringSetAsync(key, value, expiry);
    }

    public async Task DeleteAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public void Dispose()
    {
        _redis?.Dispose();
    }
}
