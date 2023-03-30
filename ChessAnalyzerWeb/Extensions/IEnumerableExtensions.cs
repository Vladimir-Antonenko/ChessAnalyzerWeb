namespace ChessAnalyzerApi.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }

    public static bool Anybody<T>(this IEnumerable<T> source)
    {
        return source?.Any() ?? false;
    }
}