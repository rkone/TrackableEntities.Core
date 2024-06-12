using System.Collections;
using System.Reflection;
using TrackableEntities.Common.Core;

namespace TrackableEntities.Client.Core;
/// <summary>
/// This class traverses a ITrackable collection, storing all ITrackable entities that have changes or reference other ITrackable entities with changes.
/// </summary>
internal class CloneChangeHelper
{
    private readonly ObjectVisitationHelper visitationHelper = new();
    private readonly Dictionary<ITrackable, EntityChangedInfo> entityChangedInfos = new(ObjectReferenceEqualityComparer<ITrackable>.Default);
    private class EntityChangedInfo
    {
        public readonly HashSet<PropertyInfo> RefNavPropUnchanged = [];

        public readonly Dictionary<PropertyInfo, HashSet<ITrackable>> ColNavPropChangedEntities = [];
    }

    private EntityChangedInfo EntityInfo(ITrackable entity)
    {
        if (!entityChangedInfos.TryGetValue(entity, out EntityChangedInfo? info))
        {
            info = new EntityChangedInfo();
            entityChangedInfos.Add(entity, info);
        }

        return info;
    }

    /// <summary>
    /// Get entities that have been added, modified or deleted, including trackable 
    /// reference and child entities.
    /// </summary>
    /// <param name="items">Collection of ITrackable objects</param>
    /// <returns>Collection containing only added, modified or deleted entities</returns>
    public IEnumerable<ITrackable> GetChanges(IEnumerable<ITrackable> items)
    {
        // Prevent endless recursion by collection
        if (!visitationHelper.TryVisit(items)) yield break;

        // Prevent endless recursion by item
        items = items.Where(i => visitationHelper.TryVisit(i)).ToList();

        // Iterate items in change-tracking collection
        foreach (ITrackable item in items)
        {
            // Downstream changes flag
            bool hasDownstreamChanges = false;

            // Iterate entity properties
            foreach (var navProp in item.GetNavigationProperties())
            {
                // Process 1-1 and M-1 properties
                foreach (var refProp in navProp.AsReferenceProperty())
                {
                    if (refProp.EntityReference is null || refProp.Property is null) continue;
                    ITrackable trackableRef = refProp.EntityReference;

                    // if already visited and unchanged, set to null
                    if (visitationHelper.IsVisited(trackableRef))
                    {
                        if (trackableRef.TrackingState == TrackingState.Unchanged)
                        {
                            EntityInfo(item).RefNavPropUnchanged.Add(refProp.Property);
                        }

                        continue;
                    }

                    // Get changed ref prop
                    ITrackingCollection? refChangeTracker = item.GetRefPropertyChangeTracker(refProp.Property?.Name);
                    if (refChangeTracker is null || refProp.Property is null) continue;

                    // Get downstream changes
                    IEnumerable<ITrackable> refPropItems = refChangeTracker.Cast<ITrackable>();
                    IEnumerable<ITrackable> refPropChanges = GetChanges(refPropItems);

                    // Set flag for downstream changes
                    bool hasLocalDownstreamChanges =
                        refPropChanges.Any(t => t.TrackingState != TrackingState.Deleted) ||
                        trackableRef.TrackingState == TrackingState.Added ||
                        trackableRef.TrackingState == TrackingState.Modified;

                    // Set ref prop to null if unchanged
                    if (!hasLocalDownstreamChanges && trackableRef.TrackingState == TrackingState.Unchanged)
                    {
                        EntityInfo(item).RefNavPropUnchanged.Add(refProp.Property);
                        continue;
                    }

                    // prevent overwrite of hasDownstreamChanges when return from recursion
                    hasDownstreamChanges |= hasLocalDownstreamChanges;
                }

                // Process 1-M and M-M properties
                foreach (var colProp in navProp.AsCollectionProperty<IList>())
                {
                    if (colProp.EntityCollection is null || colProp.Property is null) continue;
                    // Get changes on child collection
                    var trackingItems = colProp.EntityCollection;
                    if (trackingItems.Count > 0)
                    {
                        // Continue recursion if trackable hasn't been visited
                        if (!visitationHelper.IsVisited(trackingItems))
                        {
                            // Get changes on child collection
                            var trackingCollChanges = new HashSet<ITrackable>(
                                GetChanges(trackingItems.Cast<ITrackable>()),
                                ObjectReferenceEqualityComparer<ITrackable>.Default);

                            // Set flag for downstream changes
                            hasDownstreamChanges |= trackingCollChanges.Count != 0;

                            // Memorize only changed items of collection
                            EntityInfo(item).ColNavPropChangedEntities[colProp.Property] = trackingCollChanges;
                        }
                    }
                }
            }

            // Return item if it has changes
            if (hasDownstreamChanges || item.TrackingState != TrackingState.Unchanged)
                yield return item;
        }
    }

    /// <summary>
    /// After GetChanges has been called, this method can be used to determine if a 
    /// reference navigation property should be included while cloning changes only.
    /// </summary>
    /// <param name="entity">The ITrackable we are deciding if we should include</param>
    /// <param name="propertyInfo">The entity's reference property to inspect</param>
    /// <returns>True if entity should be included in the collection with changes</returns>
    public bool IncludeReferenceProp(ITrackable entity, PropertyInfo propertyInfo)
    {
        if (!entityChangedInfos.TryGetValue(entity, out EntityChangedInfo? info))
            return true; // no excludes found for this entity

        return !info.RefNavPropUnchanged.Contains(propertyInfo);
    }

    /// <summary>
    /// After GetChanges has been called, this method can be used to determine if a
    /// collection item should be included while cloning changes only.
    /// </summary>
    /// <param name="entity">The parent ITrackable that contains the collection</param>
    /// <param name="propertyInfo">The Collection Reference property</param>
    /// <param name="item">An item in the collection</param>
    /// <returns>True if entity should be included in the collection with changes</returns>
    public bool IncludeCollectionItem(ITrackable entity, PropertyInfo propertyInfo, ITrackable item)
    {
        if (!entityChangedInfos.TryGetValue(entity, out EntityChangedInfo? info))
            return true; // no excludes found for this entity

        if (!info.ColNavPropChangedEntities.TryGetValue(propertyInfo, out HashSet<ITrackable>? changedItems))
            return false; // no items found for this collection

        return changedItems.Contains(item);
    }
}
