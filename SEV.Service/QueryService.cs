using SEV.Domain.Model;
using SEV.Domain.Services;
using SEV.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SEV.Service
{
    internal class QueryService : Service, IQueryService
    {
        public QueryService(IUnitOfWorkFactory factory) : base(factory)
        {
        }

        public IEnumerable<T> Read<T>(params Expression<Func<T, object>>[] includes) where T : Entity
        {
            IEnumerable<T> entities;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                entities = unitOfWork.Repository<T>().All();

// ReSharper disable PossibleMultipleEnumeration
                if (HasToLoadRelationships(entities, includes))
                {
                    unitOfWork.RelationshipsLoader<T>().Load(entities, includes);
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
                    unitOfWork.RelationshipsLoader<T>().Load(entity, includes);
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
                    unitOfWork.RelationshipsLoader<T>().Load(entities, includes);
                }
            }

            return entities;
        }

        public IEnumerable<T> FindByQuery<T>(IQuery<T> query) where T : Entity
        {
            IList<T> entities;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                var repositoryQuery = unitOfWork.Repository<T>().Query();

                if (query.Filter != null)
                {
                    repositoryQuery = repositoryQuery.Filter(query.Filter);
                }
                if (query.Ordering != null)
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
                    if (entities.Count == 1)
                    {
                        unitOfWork.RelationshipsLoader<T>().Load(entities.Single(), query.Includes);
                    }
                    else
                    {
                        unitOfWork.RelationshipsLoader<T>().Load(entities, query.Includes);
                    }
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

        public async Task<IEnumerable<T>> ReadAsync<T>(params Expression<Func<T, object>>[] includes) where T : Entity
        {
            IEnumerable<T> entities;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                entities = await unitOfWork.Repository<T>().AllAsync();

                if (HasToLoadRelationships(entities, includes))
                {
                    unitOfWork.RelationshipsLoader<T>().Load(entities, includes);
                }
            }

            return entities;
        }

        public async Task<T> FindByIdAsync<T>(string entityId, params Expression<Func<T, object>>[] includes)
            where T : Entity
        {
            T entity;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                entity = await unitOfWork.Repository<T>().GetByIdAsync(entityId);

                if (HasToLoadRelationships(entity, includes))
                {
                    unitOfWork.RelationshipsLoader<T>().Load(entity, includes);
                }
            }

            return entity;
        }

        public async Task<IEnumerable<T>> FindByIdListAsync<T>(IEnumerable<string> entityIds,
            params Expression<Func<T, object>>[] includes) where T : Entity
        {
            IEnumerable<T> entities;

            using (IUnitOfWork unitOfWork = CreateUnitOfWork())
            {
                entities = await unitOfWork.Repository<T>().GetByIdListAsync(entityIds);

                if (HasToLoadRelationships(entities, includes))
                {
                    unitOfWork.RelationshipsLoader<T>().Load(entities, includes);
                }
            }

            return entities;
        }
// ReSharper restore PossibleMultipleEnumeration

        public Task<IEnumerable<T>> FindByQueryAsync<T>(IQuery<T> query) where T : Entity
        {
            return Task.Run(() => FindByQuery(query));
        }
    }
}