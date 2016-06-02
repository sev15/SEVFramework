using System;

namespace SEV.DAL.EF
{
    internal class EFRepositoryFactory : IRepositoryFactory
    {
        public dynamic Create(Type objectType, dynamic objectContext)
        {
            Type repositoryType = typeof(EFRepository<>).MakeGenericType(objectType);

            return Activator.CreateInstance(repositoryType, objectContext);
        }
    }
}