using System;

namespace SEV.Domain.Repository
{
    public class EntityActionEventArgs<TEntity> : EventArgs
        where TEntity : class
    {
        public TEntity Entity { get; private set; }
        public object Context { get; private set; }

        public EntityActionEventArgs(TEntity entity, object context)
        {
            Entity = entity;
            Context = context;
        }
    }
}