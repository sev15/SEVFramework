using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SEV.Domain.Repository
{
    public interface IRelationshipManager<TEntity> where TEntity : class
    {
        void Load(TEntity entity, IEnumerable<Expression<Func<TEntity, object>>> includes);
        void Load(IEnumerable<TEntity> entities, IEnumerable<Expression<Func<TEntity, object>>> includes);
        void CreateRelatedEntities(TEntity entityToCreate, TEntity createdEntity);
        void UpdateRelatedEntities(TEntity entity);
    }
}