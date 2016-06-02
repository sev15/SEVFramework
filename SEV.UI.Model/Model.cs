using SEV.Domain.Model;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SEV.UI.Model
{
    public abstract class Model<TEntity> : IModel
        where TEntity : Entity
    {
        protected bool IsInitialized { get; set; }

        protected Expression<Func<TEntity, object>> ParentEntityExpression { get; set; }

        protected IQueryService QueryService { get; private set; }

        protected Model(IQueryService queryService)
        {
            QueryService = queryService;
            IsInitialized = false;
        }

        protected virtual List<Expression<Func<TEntity, object>>> GetDefaultIncludes()
        {
            var includes = new List<Expression<Func<TEntity, object>>>();
            if (ParentEntityExpression != null)
            {
                includes.Add(ParentEntityExpression);
            }

            return includes;
        }

        public abstract bool IsValid { get; }

        public abstract void Load(string id);
    }
}