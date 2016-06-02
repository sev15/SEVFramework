using System.Collections.Generic;

namespace SEV.Domain.Repository
{
    internal class DomainQuery : IDomainQuery
    {
        private readonly Dictionary<string, dynamic> m_parameters;

        public DomainQuery()
        {
            m_parameters = new Dictionary<string, dynamic>();
        }

        public dynamic this[string key]
        {
            get
            {
                return m_parameters[key];
            }
            set
            {
                m_parameters[key] = value;
            }
        }
    }
}