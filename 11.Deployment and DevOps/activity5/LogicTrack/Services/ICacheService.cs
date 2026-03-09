using Microsoft.Extensions.Caching.Memory;

namespace LogicTrack.Services
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, MemoryCacheEntryOptions? options = null);
        int GetVersion(string key);
        int IncrementVersion(string key);
        string MakeVersionedKey(string baseKey, params (string, string)[] parts);
    }
}
