using Newtonsoft.Json;

namespace TrackableEntities.Client.Core.Tests.Entities.NorthwindModels;

[JsonObject(IsReference = true)]
public class PriorityOrder : Order
{
    private string _priorityPlan = string.Empty;
    public string PriorityPlan
    {
        get { return _priorityPlan; }
        set
        {
            if (value == _priorityPlan) return;
            _priorityPlan = value;
            NotifyPropertyChanged(() => PriorityPlan);
        }
    }
}
