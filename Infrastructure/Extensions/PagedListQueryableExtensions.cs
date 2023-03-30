using Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions;

public static class PagedListQueryableExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int indexFrom = 0, CancellationToken cancellationToken = default)
    {
        if (indexFrom > pageIndex)
        {
            throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
        }

        var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await source.Skip((pageIndex - indexFrom) * pageSize)
                                .Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

        return new PagedList<T>(items, count, pageIndex, pageSize);
    }
}