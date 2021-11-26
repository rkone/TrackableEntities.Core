using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core
{
    /// <summary>
    /// Extension methods for classes implementing ITrackable.
    /// </summary>
    public static class TrackableExtensions
    {
        /// <summary>
        /// Convert TrackingState to EntityState.
        /// </summary>
        /// <param name="state">Trackable entity state</param>
        /// <returns>EF entity state</returns>
        public static EntityState ToEntityState(this TrackingState state)
        {
            return state switch
            {
                TrackingState.Added => EntityState.Added,
                TrackingState.Modified => EntityState.Modified,
                TrackingState.Deleted => EntityState.Deleted,
                _ => EntityState.Unchanged,
            };
        }

        /// <summary>
        /// Convert EntityState to TrackingState.
        /// </summary>
        /// <param name="state">EF entity state</param>
        /// <returns>Trackable entity state</returns>
        public static TrackingState ToTrackingState(this EntityState state)
        {
            return state switch
            {
                EntityState.Added => TrackingState.Added,
                EntityState.Modified => TrackingState.Modified,
                EntityState.Deleted => TrackingState.Deleted,
                _ => TrackingState.Unchanged,
            };
        }
    }
}
