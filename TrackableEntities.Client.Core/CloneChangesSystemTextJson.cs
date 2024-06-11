using TrackableEntities.Common.Core;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TrackableEntities.Client.Core;

internal class CloneChangesSystemTextJson<TEntity> where TEntity : class, ITrackable, INotifyPropertyChanged
{
    public static ChangeTrackingCollection<TEntity> GetChanges(ChangeTrackingCollection<TEntity> source)
    {
        var wrapper = new Wrapper { Result = source };
        var helper = new CloneChangeHelper();
        // Inspect the graph and collect entityChangedInfos
        _ = helper.GetChanges([wrapper]).ToList();

        // Clone only changed items
        var cloner = new CloneLibrarySystemTextJson();
        return cloner.CloneChanges(wrapper, helper).Result;        
    }

    private class Wrapper : ITrackable
    {
        [JsonInclude] public ChangeTrackingCollection<TEntity> Result { get; set; } = [];

        public TrackingState TrackingState { get; set; }

        public ICollection<string>? ModifiedProperties { get; set; }
    } 
}
