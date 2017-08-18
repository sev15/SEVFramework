using SEV.Domain.Model;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SEV.UI.Model
{
    public abstract class EditableModel<TEntity> : SingleModel<TEntity>, IEditableModel
        where TEntity : Entity, new()
    {
        protected ICommandService CommandService { get; }

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

        protected void SetValue<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
// ReSharper disable AssignNullToNotNullAttribute
            var propInfo = GetPropertyInfo(propertyName);

            var oldValue = (TValue)propInfo.GetValue(Entity);

            if (!Equals(oldValue, value))
            {
                propInfo.SetValue(Entity, value);
                OnPropertyChanged(propertyName);
            }
        }

        protected void SetReference<TModel, TValue>(TModel value, [CallerMemberName] string propertyName = null)
            where TModel : class, ISingleModel
            where TValue : Entity
        {
            if (value != null && !value.IsValid)
            {
                throw new InvalidOperationException(Resources.AssignInvalidModelMsg);
            }

            TModel oldValue = FindOrCreateReference<TModel, TValue>(propertyName);

            if (!Equals(oldValue, value))
            {
                References.Remove(propertyName);
// ReSharper restore AssignNullToNotNullAttribute

                TValue entityValue = null;
                if (value != null)
                {
                    References.Add(propertyName, value);
                    entityValue = ((SingleModel<TValue>)(object)value).ToEntity();
                }

                GetPropertyInfo(propertyName).SetValue(Entity, entityValue);
                OnPropertyChanged(propertyName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}