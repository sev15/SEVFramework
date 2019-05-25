using System;
using SEV.Domain.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SEV.DAL.EF
{
    internal class ReferenceContainer : IReferenceContainer
    {
        private readonly Dictionary<PropertyInfo, object> m_references;
        private List<PropertyInfo> m_relationshipInfos;
        private List<PropertyInfo> m_childCollectionInfos;
        private List<PropertyInfo> m_childRelationshipInfos;

        public ReferenceContainer()
        {
            m_references = new Dictionary<PropertyInfo, object>();
        }

        public void AnalyzeReferences<TEntity>(TEntity entity) where TEntity : Entity
        {
            m_relationshipInfos = new List<PropertyInfo>();
            m_childCollectionInfos = new List<PropertyInfo>();
            m_childRelationshipInfos = new List<PropertyInfo>();
            PropertyInfo[] propInfos = typeof(TEntity).GetProperties();

            foreach (var propInfo in propInfos)
            {
                var propValue = propInfo.GetValue(entity);

                if (IsEntityTypeProperty(propInfo))
                {
                    m_relationshipInfos.Add(propInfo);
                }
                else
                {
                    if (propValue is ICollection)
                    {
                        m_references.Add(propInfo, propValue);

                        Type childType;
                        PropertyInfo parentPropInfo;
                        if ((entity is IAggregateRoot) &&
                            IsChildCollectionProperty(propValue, entity, out childType, out parentPropInfo))
                        {
                            m_childCollectionInfos.Add(propInfo);
                            propInfo.SetValue(entity, null);
                            m_childRelationshipInfos.AddRange(childType.GetProperties().Where(IsEntityTypeProperty));
                            EnsureParentPropertyValueIsSet((ICollection)propValue, parentPropInfo, entity);
                        }
                        else
                        {
                            propInfo.SetValue(entity, null);
                        }
                    }
                }
            }
        }

        private bool IsEntityTypeProperty(PropertyInfo propInfo)
        {
            return propInfo.PropertyType.IsSubclassOf(typeof(Entity));
        }

        private bool IsChildCollectionProperty<TEntity>(object propValue, TEntity entity, out Type childType,
            out PropertyInfo parentPropInfo) where TEntity : Entity
        {
            childType = propValue.GetType().GenericTypeArguments[0];
            parentPropInfo = childType.GetProperties().SingleOrDefault(p =>
                                                        p.GetCustomAttributes(false).Any(a => a is ParentAttribute));
            return (parentPropInfo != null) && (parentPropInfo.PropertyType == entity.GetType());
        }

        private void EnsureParentPropertyValueIsSet<TEntity>(ICollection children, PropertyInfo parentPropInfo,
            TEntity parentEntity) where TEntity : Entity
        {
            foreach (var child in children)
            {
                if (parentPropInfo.GetValue(child) == null)
                {
                    parentPropInfo.SetValue(child, parentEntity);
                }
            }
        }

        public PropertyInfo[] GetRelationships()
        {
            return m_relationshipInfos.ToArray();
        }

        public IDictionary<PropertyInfo, ICollection> GetChildCollections<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            var childCollections = new Dictionary<PropertyInfo, ICollection>();
            foreach (PropertyInfo propInfo in m_childCollectionInfos)
            {
                var propValue = m_references[propInfo];
                childCollections.Add(propInfo, (ICollection)propValue);
                m_references.Remove(propInfo);
            }
            return childCollections;
        }

        public PropertyInfo[] GetChildRelationships()
        {
            return m_childRelationshipInfos.ToArray();
        }

        public void RestoreReferences<TEntity>(TEntity entity) where TEntity : Entity
        {
            foreach (var reference in m_references)
            {
                reference.Key.SetValue(entity, reference.Value);
            }
        }
    }
}