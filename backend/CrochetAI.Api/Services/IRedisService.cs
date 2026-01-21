namespace CrochetAI.Api.Services;

public interface IRedisService
{
    Task<bool> IsConnectedAsync();
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value, TimeSpan? expiry = null);
    Task DeleteAsync(string key);
    Task<bool> KeyExistsAsync(string key);
}
