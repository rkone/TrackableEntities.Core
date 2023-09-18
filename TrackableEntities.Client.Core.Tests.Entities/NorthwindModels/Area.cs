using Newtonsoft.Json;

namespace TrackableEntities.Client.Core.Tests.Entities.NorthwindModels;

[JsonObject(IsReference = true)]
public class Area : EntityBase
{
    private int _areaId;
    public int AreaId
    {
        get { return _areaId; }
        set
        {
            if (value == _areaId) return;
            _areaId = value;
            NotifyPropertyChanged(() => AreaId);
        }
    }

    private string _areaName = string.Empty;
    public string AreaName
    {
        get { return _areaName; }
        set
        {
            if (value == _areaName) return;
            _areaName = value;
            NotifyPropertyChanged(() => AreaName);
        }
    }

    private ChangeTrackingCollection<Territory> _territories = new();
    public ChangeTrackingCollection<Territory> Territories
    {
        get { return _territories; }
        set
        {
            if (Equals(value, _territories)) return;
            _territories = value;
            NotifyPropertyChanged(() => Territories);
        }
    }
}
