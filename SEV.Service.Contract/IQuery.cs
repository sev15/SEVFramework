using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SEV.Domain.Model;

namespace SEV.Service.Contract
{
    public interface IQuery<TEntity> where TEntity : Entity
    {
        Expression<Func<TEntity, bool>> Filter { get; set; }
        IDictionary<int, Tuple<Expression<Func<TEntity, dynamic>>, bool>> Ordering { get; set; }
        int? PageCount { get; set; }
        int? PageSize { get; set; }
        IList<Expression<Func<TEntity, object>>> Includes { get; set; }
    }
}