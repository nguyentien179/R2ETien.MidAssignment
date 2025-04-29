using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace mid_assignment.Infrastructure.Helper;

public static class QueryHelper
{
    // Applies filters to the query.
    public static IQueryable<T> ApplyFilters<T>(
        IQueryable<T> query,
        List<Expression<Func<T, bool>>>? filters
    )
    {
        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }
        }
        return query;
    }

    // Applies includes to the query.
    public static IQueryable<T> ApplyIncludes<T>(IQueryable<T> query, string? includeProperties)
        where T : class // Add the constraint for reference types
    {
        if (!string.IsNullOrWhiteSpace(includeProperties))
        {
            foreach (
                var includeProperty in includeProperties.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries
                )
            )
            {
                query = query.Include(includeProperty.Trim());
            }
        }

        return query;
    }

    // Applies sorting to the query.
    public static IQueryable<T> ApplySorting<T>(
        IQueryable<T> query,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy
    )
    {
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return query;
    }
}
