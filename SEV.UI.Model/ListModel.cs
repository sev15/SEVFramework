using Microsoft.Practices.ServiceLocation;
using SEV.Domain.Model;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.UI.Model
{
    public abstract class ListModel<TModel, TEntity> : Model<TEntity>, IListModel<TModel>
        where TEntity : Entity
        where TModel : ISingleModel
    {
        private readonly IParentEntityFilterProvider m_parentEntityFilterProvider;

        protected Expression<Func<TEntity, object>> ParentEntityExpression { get; set; }

        protected ListModel(IQueryService queryService, IParentEntityFilterProvider filterProvider)
            : base(queryService)
        {
            m_parentEntityFilterProvider = filterProvider;
        }

        protected ListModel(IQueryService queryService) : base(queryService)
        {
        }

        internal void SetItems(IEnumerable<TEntity> entities)
        {
            Items = entities.Select(x => x.ToModel<TModel, TEntity>()).ToList();
            IsInitialized = true;
        }

        public virtual IList<TModel> Items { get; protected set; }

        public override bool IsValid
        {
            get
            {
                return IsInitialized && Items.All(x => x.IsValid);
            }
        }

        public virtual void Load()
        {
            var entities = QueryService.Read<TEntity>();
            SetItems(entities);
        }

        public override void Load(string id)
        {
            if (ParentEntityExpression == null)
            {
                throw new InvalidOperationException("ParentEntityExpression must be specified.");
            }
            if (m_parentEntityFilterProvider == null)
            {
                throw new InvalidOperationException("ParentEntityFilterProvider must be specified.");
            }

            var query = ServiceLocator.Current.GetInstance<IQuery<TEntity>>();
            query.Filter = m_parentEntityFilterProvider.CreateFilter(ParentEntityExpression, id);
            query.Includes = GetIncludes();
            var entities = QueryService.FindByQuery(query);
            SetItems(entities);
        }

        protected override List<Expression<Func<TEntity, object>>> GetIncludes()
        {
            var includes = base.GetIncludes();

            if (ParentEntityExpression != null)
            {
                includes.Add(ParentEntityExpression);
            }

            return includes;
        }
    }
}