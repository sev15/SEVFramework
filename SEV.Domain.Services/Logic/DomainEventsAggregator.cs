using Microsoft.Practices.ServiceLocation;
using SEV.Domain.Model;
using SEV.DI;

namespace SEV.Domain.Services.Logic
{
    public class DomainEventsAggregator : IDomainEventsAggregator
    {
        public void RaiseEvent<TEntity>(DomainEventArgs<TEntity> args) where TEntity : Entity
        {
            var handler = ServiceLocator.Current.Resolve<DomainEventHandler<TEntity>>(args.Event.ToString());
            if (handler != null)
            {
                handler.Handle(args);
            }
        }
    }
}
