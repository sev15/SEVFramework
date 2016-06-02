using SEV.Domain.Model;
using SEV.UI.Model.Contract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace SEV.UI.Model
{
    public sealed class ObservableModelCollection<TModel, TEntity> : ObservableCollection<TModel>
        where TModel : ISingleModel
        where TEntity : Entity
    {
        private readonly ICollection<TEntity> m_entityCollection;

        public ObservableModelCollection(ICollection<TEntity> entityCollection, IEnumerable<TModel> modelCollection)
            : base(modelCollection)
        {
            m_entityCollection = entityCollection;
            CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var removedModel in args.OldItems)
                {
                    var removedModelEntity = ((SingleModel<TEntity>)removedModel).ToEntity();
                    var entityToRemove = m_entityCollection.First(x => x.Equals(removedModelEntity));
                    m_entityCollection.Remove(entityToRemove);
                }
            }
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var addedModel in args.NewItems)
                {
                    var addedModelEntity = ((SingleModel<TEntity>)addedModel).ToEntity();
                    m_entityCollection.Add(addedModelEntity);
                }
            }
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                m_entityCollection.Clear();
            }
        }
    }
}