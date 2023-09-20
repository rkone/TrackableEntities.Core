using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels;
[TrackableEntity]
public partial class OrderDetail : ITrackable
{
    [Key, Column(Order = 1)]
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    [ForeignKey("OrderId")]
    [TrackableEntityTrackedProperty]
    public Order? Order { get; set; }
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    [TrackableEntityTrackedProperty]
    public Product? Product { get; set; }
    public decimal UnitPrice { get; set; }
    public double Quantity { get; set; }

    [NotMapped]
    public TrackingState TrackingState { get; set; }
    [NotMapped]
    public ICollection<string>? ModifiedProperties { get; set; }
}
