
namespace SEV.UI.Model.Contract
{
    public interface IEditableModel : ISingleModel
    {
        bool IsNew { get; }

        void New();
        void Save();
        void Delete();
    }
}