using System.Collections.Generic;
using System.Linq;
using SEV.Domain.Model;

namespace SEV.Domain.Services.Logic
{
    public abstract class DomainEventHandler<TEntity> where TEntity : Entity
    {
        public virtual void Handle(DomainEventArgs<TEntity> args)
        {
            IList<BusinessProcess<TEntity>> processes = CreateBusinessProcessList(args);
            foreach (var process in processes.OrderBy(p => p.ExecutionOrder))
            {
                process.Execute();
            }
        }

        protected abstract IList<BusinessProcess<TEntity>> CreateBusinessProcessList(DomainEventArgs<TEntity> args);
    }
}
