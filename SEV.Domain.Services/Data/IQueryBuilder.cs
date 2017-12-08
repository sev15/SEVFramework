using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.Domain.Services.Data
{
    public interface IQueryBuilder<TEntity> where TEntity : class
    {
        IQueryable<TEntity> BuildQuery(Expression<Func<TEntity, bool>> filter,
            IDictionary<int, Tuple<Expression<Func<TEntity, object>>, bool>> orderBys = null,
            int? page = null, int? pageSize = null);
    }
}