
namespace SEV.UI.Model.Contract
{
    public interface IEditableModel : ISingleModel, System.ComponentModel.INotifyPropertyChanged
    {
        bool IsNew { get; }

        void New();
        void Save();
        void Delete();
    }
}