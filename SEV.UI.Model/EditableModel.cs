using SEV.Common;
using SEV.Domain.Model;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.Linq.Expressions;

namespace SEV.UI.Model
{
    public abstract class EditableModel<TEntity> : SingleModel<TEntity>, IEditableModel
        where TEntity : Entity, new()
    {
        protected ICommandService CommandService { get; private set; }

        protected EditableModel(IQueryService queryService, ICommandService commandService) : base(queryService)
        {
            CommandService = commandService;
            IsNew = false;
        }

        public bool IsNew
        {
            get;
            private set;
        }

        public virtual void New()
        {
            var entity = new TEntity();
            SetEntity(entity);
            IsNew = true;
        }

        public virtual void Save()
        {
            // TODO : implement Validation..

            if (!IsValid)
            {
                throw new InvalidOperationException("The model is not valid.");
            }

            if (IsNew)
            {
                IsInitialized = false;
                var entity = CommandService.Create(Entity);
                SetEntity(entity);
                IsNew = false;
            }
            else
            {
                IsInitialized = false;
                CommandService.Update(Entity);
                IsInitialized = true;
            }
        }

        public virtual void Delete()
        {
            // TODO : implement Validation..

            if (!IsValid)
            {
                throw new InvalidOperationException("The model is not valid.");
            }
            if (IsNew)
            {
                throw new InvalidOperationException("Can not delete a new model.");
            }

            IsInitialized = false;
            CommandService.Delete(Entity);
        }

        //protected void SetValue<TValue>(Expression<Func<TEntity, TValue>> getterExpr, TValue value)
        //{
        //    var entityProperty = LambdaExpressionHelper.GetExpressionMethod(getterExpr);

        //    var oldValue = (TValue)entityProperty.GetValue(Entity);

        //    if (!Equals(oldValue, value))
        //    {
        //        entityProperty.SetValue(Entity, value);
        //    }
        //}

        protected void SetReference<TModel, TValue>(TModel value, Expression<Func<TEntity, TValue>> getterExpr)
            where TModel : class, ISingleModel
            where TValue : Entity
        {
            var entityValue = ((SingleModel<TValue>)(object)value).ToEntity();
            var property = LambdaExpressionHelper.GetExpressionMethod(getterExpr);
            property.SetValue(Entity, entityValue);
        }
    }
}