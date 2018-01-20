using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SEV.Domain.Services.Data
{
    public class RelationshipsStripper<TEntity> : IRelationshipsStripper<TEntity> where TEntity : class
    {
        protected List<Tuple<PropertyInfo, object>> References { get; }

        public RelationshipsStripper()
        {
            References = new List<Tuple<PropertyInfo, object>>();
        }

        public virtual void Strip(TEntity entity)
        {
            var propInfos = typeof(TEntity).GetProperties();
            foreach (var propInfo in propInfos)
            {
                var propValue = propInfo.GetValue(entity);
                if ((propValue is ICollection) && (((ICollection)propValue).Count > 0))
                {
                    References.Add(new Tuple<PropertyInfo, object>(propInfo, propValue));
                }
            }
        }

        public virtual void UnStrip(TEntity entity)
        {
            foreach (var reference in References)
            {
                reference.Item1.SetValue(entity, reference.Item2);
            }
        }
    }
}
