using HotelCore.Application.Common.Models;

namespace HotelCore.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PageRequest pageRequest)
    {
        return query
            .Skip(pageRequest.Skip)
            .Take(pageRequest.Take);
    }
}
