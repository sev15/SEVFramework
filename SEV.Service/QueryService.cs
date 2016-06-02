using SEV.Domain.Model;
using SEV.Domain.Repository;
using SEV.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.Service
{
    internal class QueryService : Service, IQueryService
    {
        public QueryService(IUnitOfWorkFactory session) : base(session)
        {
        }

        public IEnumerable<T> Read<T>(params Expression<Func<T, object>>[] includes) where T : Entity
        {
            IEnumerable<T> entities;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                entities = unitOfWork.Repository<T>().All();

                if (HasToLoadRelationships(entities, includes))
                {
                    unitOfWork.RelationshipManager<T>().Load(entities, includes);
                }
            }

            return entities;
        }

        public T FindById<T>(string entityId, params Expression<Func<T, object>>[] includes) where T : Entity
        {
            T entity;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                entity = unitOfWork.Repository<T>().GetById(entityId);

                if (HasToLoadRelationships(entity, includes))
                {
                    unitOfWork.RelationshipManager<T>().Load(entity, includes);
                }
            }

            return entity;
        }

        public IEnumerable<T> FindByIdList<T>(IEnumerable<string> entityIds, params Expression<Func<T, object>>[] includes)
            where T : Entity
        {
            IEnumerable<T> entities;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                entities = unitOfWork.Repository<T>().GetByIdList(entityIds);

                if (HasToLoadRelationships(entities, includes))
                {
                    unitOfWork.RelationshipManager<T>().Load(entities, includes);
                }
            }

            return entities;
        }

        public IEnumerable<T> FindByQuery<T>(IQuery<T> query) where T : Entity
        {
            IEnumerable<T> entities;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                var repositoryQuery = unitOfWork.Repository<T>().Query();

                if (query.Filter != null)
                {
                    repositoryQuery = repositoryQuery.Filter(query.Filter);
                }
                if ((query.Ordering != null))
                {
                    foreach (var ordering in query.Ordering)
                    {
                        var order = ordering.Key;
                        var orderBy = ordering.Value.Item1;
                        bool descending = ordering.Value.Item2;
                        repositoryQuery = repositoryQuery.OrderBy(order, orderBy, descending);
                    }
                }

                if (query.PageCount.HasValue && query.PageSize.HasValue)
                {
                    entities = repositoryQuery.GetPage(query.PageCount.Value, query.PageSize.Value);
                }
                else
                {
                    entities = repositoryQuery.Get();
                }

                if (HasToLoadRelationships(entities, query.Includes))
                {
                    unitOfWork.RelationshipManager<T>().Load(entities, query.Includes);
                }
            }

            return entities;
        }

        private bool HasToLoadRelationships<T>(T entity, IEnumerable<Expression<Func<T, object>>> includes)
             where T : Entity
        {
            return (entity != null) && CanLoadRelationships(includes);
        }

        private bool HasToLoadRelationships<T>(IEnumerable<T> entities, IEnumerable<Expression<Func<T, object>>> includes)
             where T : Entity
        {
            return entities.Any() && CanLoadRelationships(includes);
        }

        private bool CanLoadRelationships<T>(IEnumerable<Expression<Func<T, object>>> includes) where T : Entity
        {
            return (includes != null) && includes.Any();
        }
    }
}