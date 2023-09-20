using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels;
[TrackableEntity]
public partial class Customer : ITrackable
{
    [Key]
    public string CustomerId { get; set; } = string.Empty;
    public string? CustomerName { get; set; }

    public string? TerritoryId { get; set; }
    [ForeignKey("TerritoryId")]
    [TrackableEntityTrackedProperty]
    public Territory? Territory { get; set; }

    public List<Order> Orders { get; set; } = new();
    [TrackableEntityTrackedProperty]
    public CustomerSetting? CustomerSetting { get; set; }
    [TrackableEntityPropertyIgnore]
    public List<CustomerAddress> CustomerAddresses { get; set; } = new();

    [NotMapped]
    public TrackingState TrackingState { get; set; }
    [NotMapped]
    public ICollection<string>? ModifiedProperties { get; set; }
}
