namespace SEV.Domain.Repository
{
    public class DomainQueryProvider
    {
        private readonly IDomainQueryHandlerFactory m_queryHandlerFactory;

        public DomainQueryProvider(IDomainQueryHandlerFactory factory)
        {
            m_queryHandlerFactory = factory;
        }

        public IDomainQuery CreateQuery()
        {
            return new DomainQuery();
        }

        public IDomainQueryHandler<TResult> CreateHandler<TResult>(string queryName)
        {
            return m_queryHandlerFactory.CreateHandler<TResult>(queryName);
        }
    }
}