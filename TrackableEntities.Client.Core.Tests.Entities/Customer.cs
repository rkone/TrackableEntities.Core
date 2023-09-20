using TrackableEntities.Client.Core;

namespace TrackableEntities.EF.Core.Tests.FamilyModels.Client;

public partial class Customer : ClientBase, IClientBase
{
    private string? _data;
    public string? Data
    {
        get { return _data; }
        set
        {
            if (value == _data) return;
            _data = value;
            NotifyPropertyChanged();
        }
    }
}
