using SEV.Domain.Model;
using System.Reflection;

namespace SEV.DAL.EF
{
    public interface IReferenceContainer
    {
        void AnalyzeReferences<TEntity>(TEntity entity) where TEntity : Entity;
        PropertyInfo[] GetRelationships();
        PropertyInfo[] GetChildCollections<TEntity>(TEntity entity) where TEntity : Entity;
        void RestoreReferences<TEntity>(TEntity entity) where TEntity : Entity;
    }
}