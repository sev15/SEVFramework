using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SEV.Domain.Services.Data
{
    public interface IRelationshipsLoader<TEntity> where TEntity : class
    {
        void Load(TEntity entity, IEnumerable<Expression<Func<TEntity, object>>> includes);
        void Load(IEnumerable<TEntity> entities, IEnumerable<Expression<Func<TEntity, object>>> includes);
    }
}