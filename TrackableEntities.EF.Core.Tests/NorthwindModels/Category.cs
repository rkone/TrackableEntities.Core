﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels;

[TrackableEntity]
public partial class Category : ITrackable
{
    [Key]
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    [TrackableEntityTrackedProperty]
    public List<Product> Products { get; set; } = [];

    [NotMapped]
    public TrackingState TrackingState { get; set; }
    [NotMapped]
    public ICollection<string>? ModifiedProperties { get; set; }
}
