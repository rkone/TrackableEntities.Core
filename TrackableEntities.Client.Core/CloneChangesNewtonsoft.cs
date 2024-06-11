using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using TrackableEntities.Common.Core;
using System.ComponentModel;

namespace TrackableEntities.Client.Core;

internal class CloneChangesNewtonsoft<TEntity> : DefaultContractResolver where TEntity : class, ITrackable, INotifyPropertyChanged
{
    private readonly CloneChangeHelper _cloneChangeHelper = new();

    public static ChangeTrackingCollection<TEntity> GetChanges(ChangeTrackingCollection<TEntity> source)
    {
        var wrapper = new Wrapper { Result = source };
        var helper = new CloneChangesNewtonsoft<TEntity>();

        // Inspect the graph and collect entityChangedInfos
        _ = helper.GetChanges([wrapper]).ToList();

        // Clone only changed items
        return CloneLibraryNewtonsoft.CloneObject(wrapper, helper).Result;
    }
    public IEnumerable<ITrackable> GetChanges(IEnumerable<ITrackable> items) 
        => _cloneChangeHelper.GetChanges(items);   

    private class Wrapper : ITrackable
    {
        [JsonProperty] public ChangeTrackingCollection<TEntity> Result { get; set; } = [];

        public TrackingState TrackingState { get; set; }

        public ICollection<string>? ModifiedProperties { get; set; }
    }
   
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        property.ShouldSerialize =
            instance =>
            {
                if (instance is not ITrackable entity) return true;

                EntityNavigationProperty? np = entity.GetNavigationProperties(false).FirstOrDefault(x => x.Property == member);
                if (np == null) return true; // not a nav prop
                if (np.ValueIsNull) return false; // nav prop is not initialized

                foreach (var rp in np.AsReferenceProperty())
                {
                    if (rp.Property is null) continue;
                    // don't serialize unchanged reference navigation props
                    return _cloneChangeHelper.IncludeReferenceProp(entity, rp.Property);
                }

                // serialize collection navigation props
                return true;
            };

        // Inject the custom IValueProvider for entity collections which
        // returns only changed items
        if (property.ValueProvider == null) throw new NullReferenceException();
        property.ValueProvider = new CollectionValueProvider(_cloneChangeHelper, member, property.ValueProvider);

        return property;
    }

    private class CollectionValueProvider(CloneChangeHelper resolver, MemberInfo member, IValueProvider valueProvider) : IValueProvider
    {
        private readonly IValueProvider _valueProvider = valueProvider;
        private readonly MemberInfo _member = member;
        private readonly CloneChangeHelper _resolver = resolver;
        private static readonly MethodInfo _genericCast;

        static CollectionValueProvider()
        {
            Func<IEnumerable<ITrackable>, object> func = CastResult<int>;
            _genericCast = PortableReflectionHelper.Instance.GetMethodInfo(func)
                .GetGenericMethodDefinition();
        }

        public void SetValue(object target, object? value)
        {
            _valueProvider.SetValue(target, value);
        }

        public object? GetValue(object target)
        {
            if (target is not ITrackable entity)
                return _valueProvider.GetValue(target);

            var cnp = entity
                .GetNavigationProperties(false)
                .OfCollectionType()
                .FirstOrDefault(x => x.Property == _member);

            if (cnp == null)
                return _valueProvider.GetValue(target); // not a collection nav prop

            if (cnp.ValueIsNull || cnp.Property is null)
                return null; // nav prop is not initialized

            var items = cnp.EntityCollection?.Where(
                i => _resolver.IncludeCollectionItem(entity, cnp.Property, i));

            return _genericCast
                .MakeGenericMethod(
                    PortableReflectionHelper.Instance.GetGenericArguments(cnp.Property.PropertyType))
                .Invoke(null, [items]);
        }

        private static object CastResult<T>(IEnumerable<ITrackable> items)
        {
            return items.Cast<T>().ToList();        
        }
    }
}
