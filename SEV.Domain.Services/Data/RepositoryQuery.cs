using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SEV.Domain.Services.Data
{
    public class RepositoryQuery<TEntity> where TEntity : class
    {
        private readonly IQueryBuilder<TEntity> m_queryBuilder;
        private Expression<Func<TEntity, bool>> m_filter;
        private IDictionary<int, Tuple<Expression<Func<TEntity, object>>, bool>> m_ordering;

        public RepositoryQuery(IQueryBuilder<TEntity> queryBuilder)
        {
            m_queryBuilder = queryBuilder;
        }

        public RepositoryQuery<TEntity> Filter(Expression<Func<TEntity, bool>> filter)
        {
            m_filter = filter;
            return this;
        }

        public RepositoryQuery<TEntity> OrderBy(int order, Expression<Func<TEntity, object>> orderBy,
            bool byDescending = false)
        {
            if (m_ordering == null)
            {
                m_ordering = new SortedDictionary<int, Tuple<Expression<Func<TEntity, object>>, bool>>();
            }
            m_ordering.Add(order, new Tuple<Expression<Func<TEntity, object>>, bool>(orderBy, byDescending));

            return this;
        }

        public IList<TEntity> Get()
        {
            return m_queryBuilder.BuildQuery(m_filter, m_ordering).ToList();
        }

        public IList<TEntity> GetPage(int page, int pageSize)
        {
            //totalCount = m_repository.Get(m_filter).Count();

            return m_queryBuilder.BuildQuery(m_filter, m_ordering, page, pageSize).ToList();
        }
    }
}