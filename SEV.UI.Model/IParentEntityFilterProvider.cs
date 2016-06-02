using System;
using System.Linq.Expressions;
using SEV.Domain.Model;

namespace SEV.UI.Model
{
    public interface IParentEntityFilterProvider
    {
        Expression<Func<TEntity, bool>> CreateFilter<TEntity>(Expression<Func<TEntity, object>> parentExpression,
            string id) where TEntity : Entity;
    }
}