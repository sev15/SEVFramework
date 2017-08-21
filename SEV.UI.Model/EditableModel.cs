using SEV.Domain.Model;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
            ValidateOnSave();
            IsInitialized = false;

            if (IsNew)
            {
                var entity = CommandService.Create(Entity);
                SetEntity(entity);
                IsNew = false;
            }
            else
            {
                CommandService.Update(Entity);
                IsInitialized = true;
            }
        }

        private void ValidateOnSave()
        {
            // TODO : implement Validation..

            if (!IsValid)
            {
                throw new InvalidOperationException("The model is not valid.");
            }
        }

        public virtual void Delete()
        {
            ValidateOnDelete();
            IsInitialized = false;
            CommandService.Delete(Entity);
        }

        private void ValidateOnDelete()
        {
            if (!IsValid)
            {
                throw new InvalidOperationException("The model is not valid.");
            }
            if (IsNew)
            {
                throw new InvalidOperationException("Can not delete a new model.");
            }
        }

        public virtual async Task SaveAsync()
        {
            ValidateOnSave();
            IsInitialized = false;

            if (IsNew)
            {
                var entity = await CommandService.CreateAsync(Entity);
                SetEntity(entity);
                IsNew = false;
            }
            else
            {
                await CommandService.UpdateAsync(Entity);
                IsInitialized = true;
            }
        }

        public virtual async Task DeleteAsync()
        {
            ValidateOnDelete();
            IsInitialized = false;
            await CommandService.DeleteAsync(Entity);
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
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}