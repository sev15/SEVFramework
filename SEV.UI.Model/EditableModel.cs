using SEV.Common;
using SEV.Domain.Model;
using SEV.Service.Contract;
using SEV.UI.Model.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SEV.UI.Model
{
    public abstract class EditableModel<TEntity> : SingleModel<TEntity>, IEditableModel
        where TEntity : Entity, new()
    {
        private bool m_hasErrors;

        protected ICommandService CommandService { get; }
        protected IValidationService ValidationService { get; }

        protected EditableModel(IQueryService qservice, ICommandService cservice, IValidationService vservice)
            : base(qservice)
        {
            CommandService = cservice;
            ValidationService = vservice;
            m_hasErrors = false;
        }

        public override bool IsValid => base.IsValid && !m_hasErrors;

        public bool IsNew { get; private set; }

        public virtual void New()
        {
            var entity = new TEntity();
            SetEntity(entity);
            IsNew = true;
        }

        public virtual void Save()
        {
            Validate();
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

        private void Validate(bool isDeleting = false)
        {
            m_hasErrors = false;
            var results = new List<ValidationResult>();

            if (!IsValid)
            {
                results.Add(new ValidationResult("The model is not valid."));
            }
            if (isDeleting && IsNew)
            {
                results.Add(new ValidationResult("Can not delete a new model."));
            }
            var domainEvent = isDeleting ? DomainEvent.Delete : IsNew ? DomainEvent.Create : DomainEvent.Update;
            var domainResults = ValidationService.ValidateEntity(Entity, domainEvent);
            if (domainResults.Any())
            {
                results.AddRange(domainResults);
            }
            if (results.Any())
            {
                m_hasErrors = true;
                throw new DomainValidationException(results);
            }
        }

        public virtual void Delete()
        {
            Validate(true);
            IsInitialized = false;
            CommandService.Delete(Entity);
        }

        public virtual async Task SaveAsync()
        {
            Validate();
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
            Validate(true);
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