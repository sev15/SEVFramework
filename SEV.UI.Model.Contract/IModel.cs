namespace SEV.UI.Model.Contract
{
    public interface IModel
    {
        bool IsValid { get; }

        void Load(string id);
    }
}