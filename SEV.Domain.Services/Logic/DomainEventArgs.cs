using System;
using SEV.Domain.Model;

namespace SEV.Domain.Services.Logic
{
    public class DomainEventArgs<TEntity> : EventArgs
        where TEntity : class
    {
        public TEntity Entity { get; }
        public DomainEvent Event { get; }
        public IUnitOfWork UnitOfWork { get; }

        public DomainEventArgs(TEntity entity, DomainEvent domainEvent, IUnitOfWork unitOfWork)
        {
            Entity = entity;
            Event = domainEvent;
            UnitOfWork = unitOfWork;
        }
    }
}