using System;
using System.Linq;
using System.Linq.Expressions;

namespace Helper.ServiceGateways;

public static class QueryableExtensions
{
    public static IQueryable<T> ConditionalWhere<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate) => condition ? query.Where(predicate) : query;
    public static IQueryable<T> ConditionalIfNotNull<T>(this IQueryable<T> query, object? value, Expression<Func<T, bool>> predicate) => value is not null ? query.Where(predicate) : query;
}