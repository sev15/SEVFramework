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
        private List<PropertyInfo> m_relationships;
        private List<PropertyInfo> m_childCollections;

        public ReferenceContainer()
        {
            m_references = new Dictionary<PropertyInfo, object>();
        }

        public void AnalyzeReferences<TEntity>(TEntity entity) where TEntity : Entity
        {
            m_relationships = new List<PropertyInfo>();
            m_childCollections = new List<PropertyInfo>();
            PropertyInfo[] propInfos = typeof(TEntity).GetProperties();

            foreach (var propInfo in propInfos)
            {
                if (IsEntityTypeProperty(propInfo))
                {
                    m_relationships.Add(propInfo);
                }
                else
                {
                    var propValue = propInfo.GetValue(entity);
                    if (propValue is ICollection)
                    {
                        if ((entity is IAggregateRoot) && IsChildCollectionProperty(propValue, entity))
                        {
                            m_childCollections.Add(propInfo);
                            m_references.Add(propInfo, propValue);
                            propInfo.SetValue(entity, null);
                        }
                        else
                        {
                            m_references.Add(propInfo, propValue);
                            //propInfo.SetValue(entity, null);
                        }
                    }
                }
            }
        }

        private bool IsEntityTypeProperty(PropertyInfo propInfo)
        {
            return propInfo.PropertyType.IsSubclassOf(typeof(Entity));
        }

        private bool IsChildCollectionProperty<TEntity>(object propValue, TEntity entity) where TEntity : Entity
        {
            var childType = propValue.GetType().GenericTypeArguments[0];
            var parentProp = childType.GetProperties().SingleOrDefault(p =>
                                                        p.GetCustomAttributes(false).Any(a => a is ParentAttribute));
            return (parentProp != null) && (parentProp.PropertyType == entity.GetType());
        }

        public PropertyInfo[] GetRelationships()
        {
            return m_relationships.ToArray();
        }

        public PropertyInfo[] GetChildCollections<TEntity>(TEntity entity) where TEntity : Entity
        {
            foreach (PropertyInfo propInfo in m_childCollections)
            {
                var propValue = m_references[propInfo];
                propInfo.SetValue(entity, propValue);
                m_references.Remove(propInfo);
            }
            return m_childCollections.ToArray();
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