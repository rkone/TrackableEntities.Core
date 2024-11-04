using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels;
[TrackableEntity]
[JsonDerivedType(typeof(Order), typeDiscriminator: "base")]
public partial class Order : ITrackable, IMergeable
{
    [Key]
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    [Column]
    public string? CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    [TrackableEntityTrackedProperty]
    public Customer? Customer { get; set; }
    [TrackableEntityTrackedProperty]
    public List<OrderDetail> OrderDetails { get; set; } = [];

    [NotMapped]
    public TrackingState TrackingState { get; set; }
    [NotMapped]
    public ICollection<string>? ModifiedProperties { get; set; }
    [NotMapped]
    public Guid EntityIdentifier { get; set; }
}
