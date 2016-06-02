using Microsoft.Practices.ServiceLocation;
using SEV.Domain.Repository;

namespace SEV.DAL.EF
{
    public class EFUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork Create()
        {
            var context = ServiceLocator.Current.GetInstance<IDbContext>();

            return new EFUnitOfWork(context);
        }
    }
}