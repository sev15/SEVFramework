namespace SEV.Domain.Services.Logic
{
    public abstract class BusinessProcess<TEntity> where TEntity : class
    {
        protected BusinessProcess(int order, TEntity entity, IUnitOfWork unitOfWork)
        {
            Entity = entity;
            ExecutionOrder = order;
            UnitOfWork = unitOfWork;
        }

        protected TEntity Entity { get; }
        protected IUnitOfWork UnitOfWork { get; }

        public int ExecutionOrder { get; }

        public abstract void Execute();
    }
}
