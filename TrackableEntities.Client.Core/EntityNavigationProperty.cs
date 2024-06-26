﻿using System.Reflection;
using TrackableEntities.Common.Core;

namespace TrackableEntities.Client.Core;

/// <summary>
/// Common base class for differnt types of entity navigation properties, such as
/// EntityReferenceProperty and EntityCollectionProperty.
/// Provides safe cast operations:
/// <code>
/// foreach (var navProp in entity.GetNavigationProperties())
/// {
///     // 1-1 and M-1 properties
///     foreach (var refProp in navProp.AsReferenceProperty())
///         DoSomething(refProp.EntityReference);
///
///     // 1-M and M-M properties
///     foreach (var colProp in navProp.AsCollectionProperty())
///         DoSomething(colProp.EntityCollection);
/// }
/// </code>
/// The two inner loops may look unusual but they actually do at most one iteration if the
/// current 'navProp' is of a requested type. This replaces a less elegant "first-cast-then-check"
/// contruction.
/// </summary>
public abstract class EntityNavigationProperty
{
    /// <summary>
    /// Property information
    /// </summary>
    public PropertyInfo? Property { get; private set; }

    /// <summary>
    /// Casts 'this' to EntityReferenceProperty&lt;TEntity&gt;. Returns an empty enumerable
    /// if 'this' is not a reference property or the entity type is incompatible.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity reference</typeparam>
    public IEnumerable<EntityReferenceProperty<TEntity>> AsReferenceProperty<TEntity>() where TEntity : class, ITrackable
    {
        if (this is not EntityReferenceProperty refProp) yield break;

        var entity = refProp.EntityReference as TEntity;
        if (entity == null && !refProp.ValueIsNull) yield break;

        yield return new EntityReferenceProperty<TEntity>(refProp.Property, entity);
    }

    /// <summary>
    /// Shortcut: casts 'this' to EntityReferenceProperty.
    /// </summary>
    public IEnumerable<EntityReferenceProperty> AsReferenceProperty()
    {
        return AsReferenceProperty<ITrackable>();
    }

    /// <summary>
    /// Casts 'this' to EntityCollectionProperty&lt;TEntityCollection&gt;. Returns an empty enumerable
    /// if 'this' is not a collection property or the collection type is incompatible.
    /// </summary>
    /// <typeparam name="TEntityCollection">Type of entity collection</typeparam>
    public IEnumerable<EntityCollectionProperty<TEntityCollection>> AsCollectionProperty<TEntityCollection>()
        where TEntityCollection : class
    {
        if (this is not EntityCollectionProperty collProp) yield break;

        var coll = collProp.EntityCollection as TEntityCollection;
        if (coll == null && !collProp.ValueIsNull) yield break;

        yield return new EntityCollectionProperty<TEntityCollection>(collProp.Property, coll);
    }

    /// <summary>
    /// Shortcut: casts 'this' to EntityCollectionProperty.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<EntityCollectionProperty> AsCollectionProperty()
    {
        return AsCollectionProperty<IEnumerable<ITrackable>>();
    }

    /// <summary>
    /// True if the property value is null
    /// </summary>
    public abstract bool ValueIsNull { get; }

    internal EntityNavigationProperty(PropertyInfo? propertyInfo)
    {
        Property = propertyInfo;
    }
}

/// <summary>
/// Represents an entity reference property (1-1 or M-1) of a default type 'ITrackable'.
/// </summary>
/// <remarks>
/// Creates a new EntityReferenceProperty.
/// </remarks>
/// <param name="propertyInfo">Property information</param>
/// <param name="entityReference">Entity reference value</param>
public class EntityReferenceProperty(PropertyInfo? propertyInfo, ITrackable? entityReference) : EntityNavigationProperty(propertyInfo)
{
    /// <summary>
    /// Entity reference value
    /// </summary>
    public ITrackable? EntityReference { get; private set; } = entityReference;

    /// <summary>
    /// True if the property value is null
    /// </summary>
    public override bool ValueIsNull
    {
        get { return EntityReference == null; }
    }
}

/// <summary>
/// Represents an entity reference property (1-1 or M-1) of type 'TEntity'.
/// </summary>
/// <typeparam name="TEntity">Type of entity reference</typeparam>
public class EntityReferenceProperty<TEntity> : EntityReferenceProperty where TEntity : ITrackable
{
    /// <summary>
    /// Entity reference value
    /// </summary>
    new public TEntity? EntityReference { get; private set; }

    internal EntityReferenceProperty(PropertyInfo? propertyInfo, TEntity? entityReference)
        : base(propertyInfo, entityReference)
    {
        EntityReference = entityReference;
    }
}

/// <summary>
/// Represents an entity collection property (1-M or M-M) of a default type 'IEnumerable&lt;ITrackable&gt;'.
/// </summary>
/// <remarks>
/// Creates a new EntityCollectionProperty.
/// </remarks>
/// <param name="propertyInfo">Property</param>
/// <param name="entityCollection">Entity collection value</param>
public class EntityCollectionProperty(PropertyInfo? propertyInfo, IEnumerable<ITrackable>? entityCollection) : EntityNavigationProperty(propertyInfo)
{
    /// <summary>
    /// Entity collection value
    /// </summary>
    public IEnumerable<ITrackable>? EntityCollection { get; private set; } = entityCollection;

    /// <summary>
    /// True if the property value is null
    /// </summary>
    public override bool ValueIsNull
    {
        get { return EntityCollection == null; }
    }
}

/// <summary>
/// Represents an entity collection property (1-M or M-M) of type 'TEntityCollection'.
/// </summary>
/// <typeparam name="TEntityCollection">Type of entity collection</typeparam>
public class EntityCollectionProperty<TEntityCollection> : EntityCollectionProperty
{
    /// <summary>
    /// Entity collection value
    /// </summary>
    new public TEntityCollection? EntityCollection { get; private set; }

    internal EntityCollectionProperty(PropertyInfo? propertyInfo, TEntityCollection? entityCollection)
        : base(propertyInfo, entityCollection as IEnumerable<ITrackable>)
    {
        EntityCollection = entityCollection;
    }
}
