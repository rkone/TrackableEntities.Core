using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels;

[TrackableEntity]
public partial class Area : ITrackable
{
    [Key]
    public int AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    [TrackableEntityTrackedProperty]
    public List<Territory> Territories { get; set; } = new();    

    [NotMapped]
    public TrackingState TrackingState { get; set; }
    [NotMapped]
    public ICollection<string>? ModifiedProperties { get; set; }
}
