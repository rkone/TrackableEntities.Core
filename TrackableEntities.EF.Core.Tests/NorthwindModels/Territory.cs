using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels;

public partial class Territory : ITrackable
{        
    [Key]
    public string TerritoryId { get; set; } = string.Empty;
    public string TerritoryDescription { get; set; } = string.Empty;
    public List<Employee> Employees { get; set; } = new();
    public List<Customer> Customers { get; set; } = new();
    public List<Area> Areas {  get; set; } = new();

    [NotMapped]
    public TrackingState TrackingState { get; set; }
    [NotMapped]
    public ICollection<string>? ModifiedProperties { get; set; }
}
