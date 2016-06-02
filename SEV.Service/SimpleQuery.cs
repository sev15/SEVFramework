using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SEV.Domain.Model;
using SEV.Service.Contract;

namespace SEV.Service
{
    internal class SimpleQuery<T> : IQuery<T> where T : Entity
    {
        public Expression<Func<T, bool>> Filter { get; set; }
        public IDictionary<int, Tuple<Expression<Func<T, dynamic>>, bool>> Ordering { get; set; }
        public int? PageCount { get; set; }
        public int? PageSize { get; set; }
        public IList<Expression<Func<T, object>>> Includes { get; set; }
    }
}