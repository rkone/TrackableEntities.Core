using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackableEntities.Common.Core;

namespace TrackableEntities.Client.Core;

/// <summary>
/// Collection responsible for tracking changes to entities.
/// </summary>
/// <typeparam name="TEntity">Trackable entity type</typeparam>
public class ChangeTrackingCollection<TEntity> : ObservableCollection<TEntity>, ITrackingCollection<TEntity>, ITrackingCollection
    where TEntity : class, ITrackable, INotifyPropertyChanged
{
    // Deleted entities cache
    private readonly Collection<TEntity> _deletedEntities = [];

    /// <summary>
    /// Event for when an entity in the collection has changed its tracking state.
    /// </summary>
    public event EventHandler? EntityChanged;

    /// <summary>
    /// Default constructor with change-tracking disabled
    /// </summary>
    public ChangeTrackingCollection() : this(false)
    {
    }

    /// <summary>
    /// Change-tracking will not begin after entities are added, 
    /// unless tracking is enabled.
    /// </summary>
    /// <param name="enableTracking">Enable tracking after entities are added</param>
    public ChangeTrackingCollection(bool enableTracking)
    {
        // Initialize excluded properties
        ExcludedProperties = [];

        // Enable or disable tracking
        Tracking = enableTracking;
    }

    /// <summary>
    /// Constructor that accepts one or more entities.
    /// Change-tracking will begin after entities are added.
    /// </summary>
    /// <param name="entities">Entities being change-tracked</param>
    public ChangeTrackingCollection(params TEntity[] entities)
        : this(entities, false)
    {
    }

    /// <summary>
    /// Constructor that accepts a collection of entities.
    /// Change-tracking will begin after entities are added, 
    /// unless tracking is disabled.
    /// </summary>
    /// <param name="entities">Entities being change-tracked</param>
    /// <param name="disableTracking">Disable tracking after entities are added</param>
    public ChangeTrackingCollection(IEnumerable<TEntity> entities, bool disableTracking = false)
    {
        // Initialize excluded properties
        ExcludedProperties = [];

        // Add items to the change tracking list
        foreach (TEntity item in entities)
        {
            Add(item);
        }

        Tracking = !disableTracking;
    }

    /// <summary>
    /// Properties to exclude from change tracking.
    /// </summary>
    public IList<string> ExcludedProperties { get; private set; }

    /// <summary>
    /// Turn change-tracking on and off.
    /// </summary>
    public bool Tracking
    {
        get => _tracking;
        set => SetTracking(value, new ObjectVisitationHelper(), false, EntityChanged);
    }

    /// <summary>
    /// For internal use.
    /// Turn change-tracking on and off with proper circular reference checking.
    /// </summary>
    public void SetTracking(bool value, ObjectVisitationHelper? visitationHelper, bool oneToManyOnly,
        EventHandler? entityChanged = null)
    {
        visitationHelper ??= new ObjectVisitationHelper();

        // Prevent endless recursion
        if (!visitationHelper.TryVisit(this)) return;

        // Get notified when an item in the collection has changed
        foreach (TEntity item in this)
        {
            // Prevent endless recursion
            if (!visitationHelper.TryVisit(item)) continue;

            // Property change notification
            if (value) item.PropertyChanged += OnPropertyChanged;
            else item.PropertyChanged -= OnPropertyChanged;

            // Enable tracking on trackable collection properties
            item.SetTracking(value, visitationHelper, oneToManyOnly, entityChanged);

            // Set entity identifier
            if (item is IIdentifiable identifiable)
                identifiable.SetEntityIdentifier();
        }

        _tracking = value;
    }

    private bool _tracking;

    /// <summary>
    /// ITrackable parent referencing items in this collection.
    /// </summary>
    public ITrackable? Parent { get; set; }

    // Fired when an item in the collection has changed
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (Tracking)
        {
            if (e.PropertyName == null)
                throw new ArgumentNullException(nameof(e));

            if (sender is not ITrackable entity) return;

            // Enable tracking on reference properties
            var prop = PortableReflectionHelper.Instance.GetProperty(entity.GetType(), e.PropertyName);
            if (prop != null &&
                PortableReflectionHelper.Instance.IsAssignable(typeof(ITrackable), prop.PropertyType))
            {
                ITrackingCollection? refPropChangeTracker = entity.GetRefPropertyChangeTracker(e.PropertyName);
                if (refPropChangeTracker != null)
                    refPropChangeTracker.Tracking = Tracking;
                return;
            }

            if (e.PropertyName != nameof(ITrackable.TrackingState)
                && e.PropertyName != nameof(ITrackable.ModifiedProperties)
                && !ExcludedProperties.Contains(e.PropertyName))
            {
                // If unchanged mark item as modified, fire EntityChanged event
                if (entity.TrackingState == TrackingState.Unchanged)
                {
                    entity.TrackingState = TrackingState.Modified;
                    EntityChanged?.Invoke(this, EventArgs.Empty);
                }

                // Add prop to modified props, and fire EntityChanged event
                if (entity.TrackingState == TrackingState.Unchanged
                    || entity.TrackingState == TrackingState.Modified)
                {
                    entity.ModifiedProperties ??= [];
                    entity.ModifiedProperties.Add(e.PropertyName);
                }
            }
        }
    }

    /// <summary>
    /// Insert item at specified index.
    /// </summary>
    /// <param name="index">Zero-based index at which item should be inserted</param>
    /// <param name="item">Item to insert</param>
    protected override void InsertItem(int index, TEntity item)
    {
        if (Tracking)
        {
            // Set entity identifier
            if (item is IIdentifiable identifiable)
                identifiable.SetEntityIdentifier();

            // Listen for property changes
            item.PropertyChanged += OnPropertyChanged;

            // Exclude this collection and Parent entity (used in M-M relationships)
            // from recursive algorithms: SetTracking and SetState.
            var visitationHelper = new ObjectVisitationHelper(Parent);
            visitationHelper.TryVisit(this);

            // Enable tracking on trackable properties
            item.SetTracking(Tracking, visitationHelper.Clone());

            // Mark item and trackable collection properties
            item.SetState(TrackingState.Added, visitationHelper.Clone());

            // Fire EntityChanged event
            EntityChanged?.Invoke(this, EventArgs.Empty);
        }

        base.InsertItem(index, item);
    }

    /// <summary>
    /// Remove item at specified index.
    /// </summary>
    /// <param name="index">Zero-based index at which item should be removed</param>
    protected override void RemoveItem(int index)
    {
        // Mark existing item as deleted, stop listening for property changes,
        // then fire EntityChanged event, and cache item.
        if (Tracking)
        {
            // Get item by index
            TEntity item = Items[index];

            // Exclude this collection and Parent entity (used in M-M relationships)
            // from recursive algorithms: SetModifiedProperties, SetTracking and SetState.
            var visitationHelper = new ObjectVisitationHelper(Parent);
            visitationHelper.TryVisit(this);

            // Remove modified properties
            item.ModifiedProperties = null;
            item.SetModifiedProperties(null, visitationHelper.Clone());

            // Stop listening for property changes
            item.PropertyChanged -= OnPropertyChanged;

            // Disable tracking on trackable properties
            item.SetTracking(false, visitationHelper.Clone(), true);

            // Mark item and trackable collection properties
            bool manyToManyAdded = Parent != null && item.TrackingState == TrackingState.Added;
            item.SetState(TrackingState.Deleted, visitationHelper.Clone());

            // Fire EntityChanged event
            EntityChanged?.Invoke(this, EventArgs.Empty);

            // Cache deleted item if not added or already cached
            if (item.TrackingState != TrackingState.Added
                && !manyToManyAdded
                && !_deletedEntities.Contains(item))
                _deletedEntities.Add(item);
        }

        base.RemoveItem(index);
    }

    /// <summary>
    /// Get entities that have been added, modified or deleted, including child 
    /// collections with entities that have been added, modified or deleted.
    /// </summary>
    /// <returns>Collection containing only changed entities</returns>
    public ChangeTrackingCollection<TEntity> GetChanges()
    {
        // Temporarily restore deletes
        this.RestoreDeletes();

        try
        {
#if SYSTEMTEXTJSON
            var wrapper = new Wrapper { Result = this };
            var cloner = new CloneLibrarySystemTextJson();
            return cloner.CloneChanges(wrapper).Result;
#else

            return CloneChangesNewtonsoft<TEntity>.GetChanges(this);
#endif
        }           
        finally
        {
            // Remove deletes
            this.RemoveRestoredDeletes();
        }
    }

    private class Wrapper : ITrackable
    {
        [JsonInclude]
        public ChangeTrackingCollection<TEntity> Result { get; set; } = [];
        public TrackingState TrackingState { get; set; }
        public ICollection<string>? ModifiedProperties { get; set; }
    }

    /// <summary>
    /// Get entities that have been added, modified or deleted.
    /// </summary>
    /// <returns>Collection containing only changed entities</returns>
    ITrackingCollection ITrackingCollection.GetChanges()
    {
        // Get changed items in this tracking collection
        var changes = (from existing in this
                       where existing.TrackingState != TrackingState.Unchanged
                       select existing)
            .Union(_deletedEntities);
        return new ChangeTrackingCollection<TEntity>(changes, true);
    }

    /// <summary>
    /// Turn change-tracking on and off without graph traversal (internal use).
    /// </summary>
    bool ITrackingCollection.InternalTracking
    {
        set { _tracking = value; }
    }

    /// <summary>
    /// Get deleted entities which have been cached.
    /// </summary>
    ICollection ITrackingCollection.CachedDeletes
    {
        get { return _deletedEntities; }
    }

    /// <summary>
    /// Remove deleted entities which have been cached.
    /// </summary>
    void ITrackingCollection.RemoveCachedDeletes()
    {
        _deletedEntities.Clear();
    }

    /// <summary>
    /// <para>Merge changed child items into the original trackable entity. 
    /// This assumes GetChanges was called to update only changed items. 
    /// Non-recursive - only direct children will be merged.</para> 
    /// <code>Usage: MergeChanges(ref originalItem, updatedItem);</code>
    /// </summary>
    /// <param name="originalItem">Local entity containing unchanged child items.</param>
    /// <param name="updatedItem">Entity returned by an update operation.</param>
    [Obsolete("ChangeTrackingCollection.MergeChanges has been deprecated. Instead use ChangeTrackingCollection.MergeChanges.")]
    public void MergeChanges(ref TEntity originalItem, TEntity updatedItem)
    {
        // Get unchanged child entities
        foreach (var colProp in updatedItem.GetNavigationProperties().OfCollectionType<IList>())
        {
            if (colProp.Property is null || colProp.EntityCollection is null) continue;
            var updatedItems = colProp.EntityCollection;
            var originalItems = originalItem.GetEntityCollectionProperty(colProp.Property).EntityCollection;
            if (originalItems != null)
            {
                foreach (ITrackable origTrackable in originalItems)
                {
                    if (origTrackable.TrackingState == TrackingState.Unchanged)
                    {
                        // Add unchanged original item to updated items
                        updatedItems.Add(origTrackable);
                        FixUpParentReference(origTrackable, updatedItem, Tracking);
                    }
                }
            }
        }

        // Track updated item
        bool tracking = Tracking;
        Tracking = false;
        Remove(originalItem);
        Add(updatedItem);
        Tracking = tracking;

        // Set original item to updated item with unchanged items merged in
        originalItem = updatedItem;
    }
    private void FixUpParentReference(ITrackable child, ITrackable parent, bool isTracking)
    {
        foreach (var refProp in child.GetNavigationProperties()
            .OfReferenceType()
            .Where(rp => rp.Property != null && PortableReflectionHelper.Instance.IsAssignable(rp.Property.PropertyType, parent.GetType()))
            .Where(rp => !ReferenceEquals(rp.EntityReference, parent)))
        {
            Tracking = false;
            refProp.Property?.SetValue(child, parent, null);
            Tracking = isTracking;
        }
    }
}