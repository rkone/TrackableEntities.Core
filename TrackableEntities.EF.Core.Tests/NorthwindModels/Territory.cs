using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels;
[TrackableEntity]
public partial class Territory : ITrackable
{        
    [Key]
    public string TerritoryId { get; set; } = string.Empty;
    public string TerritoryDescription { get; set; } = string.Empty;
    [TrackableEntityTrackedProperty(true)]
    public List<Employee> Employees { get; set; } = [];
    [TrackableEntityTrackedProperty]
    public List<Customer> Customers { get; set; } = [];
    [TrackableEntityTrackedProperty]
    public List<Area> Areas { get; set; } = [];

    [NotMapped]
    public TrackingState TrackingState { get; set; }
    [NotMapped]
    public ICollection<string>? ModifiedProperties { get; set; }
}
