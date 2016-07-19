using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.Domain.Repository
{
    public abstract class QueryBuilder<TEntity> : IQueryBuilder<TEntity> where TEntity : class
    {
        protected abstract IQueryable<TEntity> CreateQuery();

        public IQueryable<TEntity> BuildQuery(Expression<Func<TEntity, bool>> filter,
            IDictionary<int, Tuple<Expression<Func<TEntity, object>>, bool>> orderBys = null,
            int? page = null, int? size = null)
        {
            var query = CreateQuery();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBys != null)
            {
                query = ApplyOrderBys(query, orderBys);
            }

            if (!page.HasValue && !size.HasValue)
            {
                return query;
            }

            if ((page.HasValue && !size.HasValue) || !page.HasValue)
            {
                throw new InvalidOperationException("The page and size parameters must be specified when querying by page.");
            }

            if (orderBys == null)
            {
                throw new InvalidOperationException("The orderBys parameter must be specified when querying by page.");
            }

            query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);

            return query;
        }

        private IQueryable<TEntity> ApplyOrderBys(IQueryable<TEntity> query,
            IDictionary<int, Tuple<Expression<Func<TEntity, object>>, bool>> orderBys)
        {
            var orders = orderBys.Keys.ToArray();

            var orderByPair1 = orderBys[orders[0]];
            var orderedQuery = ApplyFirstOrderBy(query, orderByPair1.Item1, orderByPair1.Item2);

            for (int i = 1; i < orders.Length; i++)
            {
                var orderByPair = orderBys[orders[i]];
                orderedQuery = ApplyOrderBy(orderedQuery, orderByPair.Item1, orderByPair.Item2);
            }

            return orderedQuery;
        }

        private IOrderedQueryable<TEntity> ApplyFirstOrderBy(IQueryable<TEntity> query,
            Expression<Func<TEntity, object>> orderBy, bool byDescending)
        {
            if (orderBy.Body is MemberExpression)
            {
                return byDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }

            if (!ValidateOrderBy(orderBy))
            {
                throw new ArgumentException("Invalid orderBy parameter", "orderBy");
            }

            var lambda = RebuildOrderBy(orderBy);
            var lambdaType = lambda.Body.Type;

            if (lambdaType == typeof(int))
            {
                return ApplyFirstOrderBy<int>(query, lambda, byDescending);
            }
            if (lambdaType == typeof(DateTime))
            {
                return ApplyFirstOrderBy<DateTime>(query, lambda, byDescending);
            }
            if (lambdaType == typeof(DateTime?))
            {
                return ApplyFirstOrderBy<DateTime?>(query, lambda, byDescending);
            }
            //if (lambdaType == typeof(Guid))
            //{
            //    return ApplyFirstOrderBy<Guid>(query, lambda, byDescending);
            //}
            if (lambdaType == typeof(decimal))
            {
                return ApplyFirstOrderBy<decimal>(query, lambda, byDescending);
            }
            throw new ArgumentException("Unsupported type of orderBy property", "orderBy");
        }

        private bool ValidateOrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            var nodeType = orderBy.Body.NodeType;

            return (nodeType == ExpressionType.Convert) || (nodeType == ExpressionType.ConvertChecked);
        }

        private LambdaExpression RebuildOrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            var param = Expression.Parameter(typeof(TEntity), "x");
            var operandExpr = (MemberExpression)((UnaryExpression)orderBy.Body).Operand;
            var member = operandExpr.Member;
            if (typeof(TEntity).GetProperty(member.Name) == null)
            {
                member = ((MemberExpression)operandExpr.Expression).Member;
            }
            return Expression.Lambda(Expression.MakeMemberAccess(param, member), param);
        }

        private IOrderedQueryable<TEntity> ApplyFirstOrderBy<T>(IQueryable<TEntity> query, LambdaExpression orderBy,
            bool byDescending)
        {
            var typedOrderBy = (Expression<Func<TEntity, T>>)orderBy;
            return byDescending ? query.OrderByDescending(typedOrderBy) : query.OrderBy(typedOrderBy);
        }

        private IOrderedQueryable<TEntity> ApplyOrderBy(IOrderedQueryable<TEntity> query,
            Expression<Func<TEntity, object>> orderBy, bool byDescending)
        {
            if (orderBy.Body is MemberExpression)
            {
                return byDescending ? query.ThenByDescending(orderBy) : query.ThenBy(orderBy);
            }

            if (!ValidateOrderBy(orderBy))
            {
                throw new ArgumentException("Invalid orderBy parameter", "orderBy");
            }

            var lambda = RebuildOrderBy(orderBy);
            var lambdaType = lambda.Body.Type;

            if (lambdaType == typeof(int))
            {
                return ApplyOrderBy<int>(query, lambda, byDescending);
            }
            if (lambdaType == typeof(DateTime))
            {
                return ApplyOrderBy<DateTime>(query, lambda, byDescending);
            }
            if (lambdaType == typeof(DateTime?))
            {
                return ApplyOrderBy<DateTime?>(query, lambda, byDescending);
            }
            //if (lambdaType == typeof(Guid))
            //{
            //    return ApplyOrderBy<Guid>(query, lambda, byDescending);
            //}
            if (lambdaType == typeof(decimal))
            {
                return ApplyOrderBy<decimal>(query, lambda, byDescending);
            }
            throw new ArgumentException("Unsupported type of orderBy property", "orderBy");
        }

        private IOrderedQueryable<TEntity> ApplyOrderBy<T>(IOrderedQueryable<TEntity> query, LambdaExpression orderBy,
            bool byDescending)
        {
            var typedOrderBy = (Expression<Func<TEntity, T>>)orderBy;
            return byDescending ? query.ThenByDescending(typedOrderBy) : query.ThenBy(typedOrderBy);
        }
    }
}