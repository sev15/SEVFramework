using SEV.Domain.Model;

namespace SEV.Domain.Services.Logic
{
    public interface IDomainEventsAggregator
    {
        void RaiseEvent<TEntity>(DomainEventArgs<TEntity> args) where TEntity : Entity;
    }
}
