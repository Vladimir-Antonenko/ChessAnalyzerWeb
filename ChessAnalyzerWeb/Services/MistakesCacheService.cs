using Microsoft.Extensions.Caching.Memory;

namespace ChessAnalyzerApi.Services
{
    public sealed class MistakesCacheService : IMemoryCacheService
    {
        public IMemoryCache Cache { get; }
        public MistakesCacheService(IMemoryCache cache)
        {
            Cache = cache;
        }
    }
}