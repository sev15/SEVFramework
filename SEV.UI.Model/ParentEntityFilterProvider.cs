using System;
using System.Linq.Expressions;
using SEV.Domain.Model;

namespace SEV.UI.Model
{
    public class ParentEntityFilterProvider : IParentEntityFilterProvider
    {
        public Expression<Func<TEntity, bool>> CreateFilter<TEntity>(Expression<Func<TEntity, object>> parentExpression,
            string id) where TEntity : Entity
        {
            var param = parentExpression.Parameters[0];
            var sourceBody = parentExpression.Body;
            var newBody = Expression.Equal(BuildLeftExpression(sourceBody), BuildRightExpression(id));

            return Expression.Lambda<Func<TEntity, bool>>(newBody, param);
        }

        private Expression BuildLeftExpression(Expression body)
        {
            var bodyType = body.Type;
            var prop = bodyType.GetProperty("Id", typeof(int));

            return Expression.Property(body, prop);
        }

        private Expression BuildRightExpression(string id)
        {
            var parentEntityId = Int32.Parse(id);
            return Expression.Constant(parentEntityId, typeof(int));
        }
    }
}