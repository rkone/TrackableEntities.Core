using TrackableEntities.Client.Core;

namespace TrackableEntities.EF.Core.Tests.FamilyModels.Client;

public partial class Territory : ClientBase, IClientBase
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

    private int _areaId;
    public int AreaId
    {
        get { return _areaId; }
        set
        {
            if (value == _areaId) return;
            _areaId = value;
            NotifyPropertyChanged();
        }
    }

    private Area? _area;
    public Area? Area
    {
        get { return _area; }
        set
        {
            if (value == _area) return;
            _area = value;
            AreaChangeTracker = _area == null ? null
                : new ChangeTrackingCollection<Area> { _area };
            NotifyPropertyChanged();
        }
    }
    private ChangeTrackingCollection<Area>? AreaChangeTracker { get; set; }
    //note: incremental generation has lost the ability to allow the ChangeTrackingCollection backing property name to be customized.  Tony's implementation used the property name "AreaChangeTracker_NON_STANDARD, and had the class implement IRefPropertyChangeTrackerResolver.
    //This allowed the backing property name to be customized.  The backing property name is now always the property name with "_ChangeTracker" appended.  This is a breaking change for anyone who customized the backing property name.
    //further changes to the incrementalgenerator would allow this, but I'll leave that up to the user to implement.
}
