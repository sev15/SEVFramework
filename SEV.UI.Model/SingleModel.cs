using SEV.Domain.Model;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SEV.UI.Model
{
    public abstract class SingleModel<TEntity> : Model<TEntity>, ISingleModel
        where TEntity : Entity
    {
        private readonly Dictionary<string, object> m_collections;

        protected TEntity Entity { get; set; }
        protected Dictionary<string, ISingleModel> References { get; }

        protected SingleModel(IQueryService queryService) : base(queryService)
        {
            References = new Dictionary<string, ISingleModel>();
            m_collections = new Dictionary<string, object>();
        }

        internal void SetEntity(TEntity entity)
        {
            Entity = entity;
            IsInitialized = entity != null;
        }

        internal TEntity ToEntity() => Entity;

        public string Id => Entity.EntityId;

        public override bool IsValid => IsInitialized && (Entity != null);

        public override void Load(string id)
        {
            var includes = GetIncludes();
            var entity = includes.Any() ? QueryService.FindById(id, includes.ToArray())
                                        : QueryService.FindById<TEntity>(id);
            SetEntity(entity);
        }

        protected TValue GetValue<TValue>(Func<TEntity, TValue> getter)
        {
            return getter(Entity);
        }

        protected TModel GetReference<TModel, TValue>([CallerMemberName] string propertyName = null)
            where TModel : class, ISingleModel
            where TValue : Entity
        {
// ReSharper disable AssignNullToNotNullAttribute
            return FindOrCreateReference<TModel, TValue>(propertyName);
        }

        protected TModel FindOrCreateReference<TModel, TValue>(string propertyName)
            where TModel : class, ISingleModel
            where TValue : Entity
        {
            ISingleModel reference;

            if (!References.TryGetValue(propertyName, out reference))
            {
                var refEntity = (TValue)GetPropertyInfo(propertyName).GetValue(Entity);
                if (refEntity != null)
                {
                    reference = refEntity.ToModel<TModel, TValue>();
                    References.Add(propertyName, reference);
                }
            }

            return (TModel)reference;
        }

        protected ObservableModelCollection<TModel, TValue> GetCollection<TModel, TValue>([CallerMemberName] string propertyName = null)
            where TModel : ISingleModel
            where TValue : Entity
        {
// ReSharper restore AssignNullToNotNullAttribute
            return FindOrCreateCollection<TModel, TValue>(propertyName);
        }

        private ObservableModelCollection<TModel, TValue> FindOrCreateCollection<TModel, TValue>(string propertyName)
            where TModel : ISingleModel
            where TValue : Entity
        {
            object collection;

            if (!m_collections.TryGetValue(propertyName, out collection))
            {
// ReSharper restore AssignNullToNotNullAttribute
                var entityCollection = (ICollection<TValue>)GetPropertyInfo(propertyName).GetValue(Entity);
                var modelCollection = entityCollection.Select(x => x.ToModel<TModel, TValue>());
                collection = new ObservableModelCollection<TModel, TValue>(entityCollection, modelCollection);
                m_collections.Add(propertyName, collection);
            }

            return (ObservableModelCollection<TModel, TValue>)collection;
        }

        protected PropertyInfo GetPropertyInfo(string propertyName)
        {
            return Entity.GetType().GetProperty(propertyName);
        }
    }
}