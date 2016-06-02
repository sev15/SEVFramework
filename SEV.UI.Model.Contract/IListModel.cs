using System.Collections.Generic;

namespace SEV.UI.Model.Contract
{
    public interface IListModel<TModel> : IModel where TModel : ISingleModel
    {
        IList<TModel> Items { get; }

        void Load();

        // TODO : add method to load by query parameters.
        //void Load(IQueryParameters<TModel> queryParameters);
    }
}