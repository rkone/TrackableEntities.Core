using System.Text.Json.Serialization;

namespace TrackableEntities.EF.Core.Tests.FamilyModels.Client;

/// <summary>
/// With System.Jext.Json, to enable polymorphic serialization, you must add a JsonDerivedType attribute to the base class.
/// </summary>
[JsonDerivedType(typeof(Order), typeDiscriminator: "base")]
[JsonDerivedType(typeof(PriorityOrder), typeDiscriminator:"Priority")]
public partial class Order{ }

