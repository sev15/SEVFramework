using Microsoft.Practices.ServiceLocation;
using SEV.Common;
using SEV.Domain.Model;
using SEV.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SEV.DI;

namespace SEV.DAL.EF
{
    public class EFRelationshipManager<TEntity> : IRelationshipManager<TEntity> where TEntity : Entity
    {
        private readonly IDbContext m_context;

        public EFRelationshipManager(IDbContext context)
        {
            m_context = context;
        }

        public void Load(TEntity entity, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            foreach (var navigationProperty in navigationProperties)
            {
                Load(entity, navigationProperty);
            }
        }

        private void Load(TEntity entity, Expression<Func<TEntity, object>> navigationProperty)
        {
            if (LambdaExpressionHelper.IsCollectionExpression(navigationProperty))
            {
                string propName = LambdaExpressionHelper.GetPropertyName(navigationProperty);
                m_context.LoadEntityCollection(entity, propName);
            }
            else
            {
                m_context.LoadEntityReference(entity, navigationProperty);
            }
        }

        public void Load(IEnumerable<TEntity> entities, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
        {
            foreach (var navigationProperty in navigationProperties)
            {
                Load(entities, navigationProperty);
            }
        }

        private void Load(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> navigationProperty)
        {
            if (LambdaExpressionHelper.IsCollectionExpression(navigationProperty))
            {
                throw new ArgumentException("Expressions for collection properties are unsupported");
            }

            var relatedEntitiesIdMap = MapRelatedEntitiesIds(navigationProperty, entities);
            var relatedEntitiesMap = MapRelatedEntities(navigationProperty, relatedEntitiesIdMap);
            AttachRelatedEntities(entities, navigationProperty, relatedEntitiesIdMap, relatedEntitiesMap);
        }

        private Dictionary<int, int> MapRelatedEntitiesIds(Expression<Func<TEntity, object>> navigationProperty, 
            IEnumerable<TEntity> entities)
        {
            string propertyName = LambdaExpressionHelper.GetPropertyName(navigationProperty);
            var map = new Dictionary<int, int>();

            foreach (var entity in entities)
            {
                var refId = m_context.GetEntityReferenceId(entity, propertyName);
                if (refId != null)
                {
                    map.Add(entity.Id, (int)refId);
                }
            }

            return map;
        }

        private Dictionary<int, Entity> MapRelatedEntities(Expression<Func<TEntity, object>> navigationProperty,
            Dictionary<int, int> idsMap)
        {
            if (!idsMap.Any())
            {
                return new Dictionary<int, Entity>();
            }

            var factory = ServiceLocator.Current.GetInstance<IRepositoryFactory>();
            var repository = factory.Create(navigationProperty.Body.Type, m_context);

            var ids = idsMap.Values.Distinct().Select(x => (object)x).ToList();
            IEnumerable<Entity> entities = repository.GetByIdList(ids);

            return entities.ToDictionary(x => x.Id);
        }

        private static void AttachRelatedEntities(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> navigationProperty, Dictionary<int, int> idsMap,
            Dictionary<int, Entity> entitiesMap)
        {
            if (!idsMap.Any())
            {
                return;
            }

            PropertyInfo propertyInfo = LambdaExpressionHelper.GetExpressionMethod(navigationProperty);
            Action<TEntity, object> relatedEntitySetter = propertyInfo.SetValue;

            foreach (var entity in entities)
            {
                if (!idsMap.ContainsKey(entity.Id))
                {
                    continue;
                }
                var relatedEntityId = idsMap[entity.Id];
                var relatedEntity = entitiesMap[relatedEntityId];
                relatedEntitySetter(entity, relatedEntity);
            }
        }

        public void CreateRelatedEntities(TEntity entityToCreate, TEntity createdEntity)
        {
            var creator = ServiceLocator.Current.Resolve<IRelatedEntitiesCreator<TEntity>>();
            if (creator == null)
            {
                return;
            }
            creator.Execute(new CreateEntityEventArgs<TEntity>(entityToCreate, m_context, createdEntity));
        }

        public void UpdateRelatedEntities(TEntity entity)
        {
            var updater = ServiceLocator.Current.Resolve<IRelatedEntitiesUpdater<TEntity>>();
            if (updater == null)
            {
                return;
            }
            updater.Execute(new EntityActionEventArgs<TEntity>(entity, m_context));
        }
    }
}