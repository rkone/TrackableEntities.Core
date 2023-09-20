using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels;
[TrackableEntity]
public partial class Employee : ITrackable
{
    [Key]
    public int EmployeeId { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    [TrackableEntityTrackedProperty(true)]
    public List<Territory> Territories { get; set; } = new();

    [NotMapped]
    public TrackingState TrackingState { get; set; }
    [NotMapped]
    public ICollection<string>? ModifiedProperties { get; set; }
}
