using SEV.Domain.Repository;

namespace SEV.Service
{
    public abstract class Service
    {
        private readonly IUnitOfWorkFactory m_uowFactory;

        protected Service(IUnitOfWorkFactory factory)
        {
            m_uowFactory = factory;
        }

        protected IUnitOfWork CreateUnitOfWork()
        {
            return m_uowFactory.Create();
        }
    }
}