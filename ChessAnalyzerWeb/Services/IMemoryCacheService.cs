using Microsoft.Extensions.Caching.Memory;

namespace ChessAnalyzerApi.Services;

public interface IMemoryCacheService
{
    IMemoryCache Cache { get; }
}