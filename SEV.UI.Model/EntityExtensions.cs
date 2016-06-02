using Microsoft.Practices.ServiceLocation;
using SEV.Domain.Model;
using SEV.UI.Model.Contract;

namespace SEV.UI.Model
{
    public static class EntityExtensions
    {
        public static TModel ToModel<TModel, TEntity>(this TEntity entity)
            where TModel : ISingleModel
            where TEntity : Entity
        {
            ISingleModel model = ServiceLocator.Current.GetInstance<TModel>();
            ((SingleModel<TEntity>)model).SetEntity(entity);

            return (TModel)model;
        }
    }
}