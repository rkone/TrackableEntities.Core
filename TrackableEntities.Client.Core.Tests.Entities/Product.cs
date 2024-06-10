using System.Text.Json.Serialization;

namespace TrackableEntities.EF.Core.Tests.FamilyModels.Client;

/// <summary>
/// With System.Jext.Json, to enable polymorphic serialization, you must add a JsonDerivedType attribute to the base class.
/// </summary>
[JsonDerivedType(typeof(Product), typeDiscriminator: "base")]
[JsonDerivedType(typeof(PromotionalProduct), typeDiscriminator:"Promo")]
public partial class Product{ }

