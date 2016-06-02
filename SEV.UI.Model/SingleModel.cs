using SEV.Domain.Model;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SEV.UI.Model
{
    public abstract class SingleModel<TEntity> : Model<TEntity>, ISingleModel
        where TEntity : Entity
    {
        protected TEntity Entity { get; set; }

        protected SingleModel(IQueryService queryService) : base(queryService)
        {
        }

        internal void SetEntity(TEntity entity)
        {
            Entity = entity;
            IsInitialized = entity != null;
        }

        internal TEntity ToEntity()
        {
            return Entity;
        }

        public string Id
        {
            get
            {
                return Entity.EntityId;
            }
        }

        public override bool IsValid
        {
            get
            {
                return IsInitialized && (Entity != null);
            }
        }

        public override void Load(string id)
        {
            var includes = GetDefaultIncludes();
            var entity = includes.Any() ? QueryService.FindById(id, includes.ToArray())
                                        : QueryService.FindById<TEntity>(id);
            SetEntity(entity);
        }

        protected TValue GetValue<TValue>(Func<TEntity, TValue> getter)
        {
            return getter(Entity);
        }

        protected TModel GetReference<TModel, TValue>(Func<TEntity, TValue> getter)
            where TModel : class, ISingleModel
            where TValue : Entity
        {
            var result = getter(Entity);

            return result != null ? result.ToModel<TModel, TValue>() : null;
        }

        protected ObservableModelCollection<TModel, TValue> GetCollection<TModel, TValue>(
            Func<TEntity, ICollection<TValue>> getter)
            where TModel : ISingleModel
            where TValue : Entity
        {
            var entityCollection = getter(Entity);
            var modelCollection = entityCollection.Select(x => x.ToModel<TModel, TValue>());

            return new ObservableModelCollection<TModel, TValue>(entityCollection, modelCollection);
        }
    }
}