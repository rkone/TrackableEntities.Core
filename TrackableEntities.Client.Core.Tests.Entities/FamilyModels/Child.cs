using Newtonsoft.Json;

namespace TrackableEntities.Client.Core.Tests.Entities.FamilyModels;

[JsonObject]
public class Child : EntityBase
{
    public Child() { }
    public Child(string name)
    {
        _name = name;
    }

    private string _name = string.Empty;
    public string Name
    {
        get { return _name; }
        set
        {
            if (value == _name) return;
            _name = value;
            NotifyPropertyChanged(() => Name);
        }
    }

    private ChangeTrackingCollection<Child> _children = new();
    public ChangeTrackingCollection<Child> Children
    {
        get { return _children; }
        set
        {
            if (Equals(value, _children)) return;
            _children = value;
            NotifyPropertyChanged(() => Children);
        }
    }
}
