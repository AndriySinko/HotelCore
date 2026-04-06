using HotelCore.Application.Common.Models;

namespace HotelCore.Application.Common.Extensions;

/// <summary>
/// Extension methods for applying pagination to IQueryable.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies pagination to a queryable using PageRequest.
    /// </summary>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PageRequest pageRequest)
    {
        return query
            .Skip(pageRequest.Skip)
            .Take(pageRequest.Take);
    }
}
