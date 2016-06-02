using System;

namespace SEV.Domain.Repository
{
    public class CreateEntityEventArgs<TEntity> : EntityActionEventArgs<TEntity>
        where TEntity : class
    {
        public TEntity CreatedEntity { get; set; }
 
        public CreateEntityEventArgs(TEntity entity, object context, TEntity createdEntity)
            : base(entity, context)
        {
            CreatedEntity = createdEntity;
        }
    }
}