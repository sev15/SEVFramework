using System;
using Microsoft.Practices.ServiceLocation;
using SEV.Domain.Model;

namespace SEV.Domain.Services.Logic
{
    public class DomainEventsAggregator : IDomainEventsAggregator
    {
        public void RaiseEvent<TEntity>(DomainEventArgs<TEntity> args) where TEntity : Entity
        {
            try
            {
                var handler = ServiceLocator.Current.GetInstance<DomainEventHandler<TEntity>>(args.Event.ToString());
                handler?.Handle(args);
            }
            catch (ActivationException)
            {
                //Do nothing when there is no handler registered for specified TEntity & DomainEvent
            }
        }
    }
}
