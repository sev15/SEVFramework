using System.Collections;
using System.Collections.Generic;
using SEV.Domain.Model;
using System.Reflection;

namespace SEV.DAL.EF
{
    public interface IReferenceContainer
    {
        void AnalyzeReferences<TEntity>(TEntity entity) where TEntity : Entity;
        PropertyInfo[] GetRelationships();
        IDictionary<PropertyInfo, ICollection> GetChildCollections<TEntity>(TEntity entity) where TEntity : Entity;
        PropertyInfo[] GetChildRelationships();
        void RestoreReferences<TEntity>(TEntity entity) where TEntity : Entity;
    }
}