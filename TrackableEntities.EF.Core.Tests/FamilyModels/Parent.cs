using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.FamilyModels;

[TrackableEntity]
public class Parent : ITrackable
{
    public Parent() { }
    public Parent(string name)
    {
        Name = name;
    }

    [Key]
    public string Name { get; set; } = string.Empty;
    public string Hobby { get; set; } = string.Empty;
    [TrackableEntityTrackedProperty]
    public List<Child> Children { get; set; } = [];

    [NotMapped]
    public TrackingState TrackingState { get; set; }
    [NotMapped]
    public ICollection<string>? ModifiedProperties { get; set; }
}
