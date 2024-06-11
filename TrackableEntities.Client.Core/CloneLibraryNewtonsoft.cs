using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using TrackableEntities.Common.Core;

namespace TrackableEntities.Client.Core;

internal static class CloneLibraryNewtonsoft
{
    public static T Clone<T>(T item) where T : class, ITrackable
    {
        return CloneObject(item);
    }

    public static IEnumerable<T> Clone<T>(IEnumerable<T> items)
    {
        return CloneObject(new CollectionSerializationHelper<T>() { Result = items.ToList() })?.Result ?? Enumerable.Empty<T>();
    }

    internal static T CloneObject<T>(T item, IContractResolver? contractResolver = null) where T : class
    {
        using var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        using var jsonWriter = new JsonTextWriter(writer);
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            ContractResolver = contractResolver ?? new EntityNavigationPropertyResolver(),
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };
        var serWr = JsonSerializer.Create(settings);
        serWr.Serialize(jsonWriter, item);
        writer.Flush();

        stream.Position = 0;
        var reader = new StreamReader(stream);
        var jsonReader = new JsonTextReader(reader);
        settings.ContractResolver = new EntityNavigationPropertyResolver();
        var serRd = JsonSerializer.Create(settings);
        var copy = serRd.Deserialize<T>(jsonReader) ?? throw new InvalidCastException();
        return copy;
    }

    private class EntityNavigationPropertyResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize =
                instance =>
                {
                    if (instance is not ITrackable entity) return true;

                    // The current property is a navigation property and its value is null
                    bool isEmptyNavProp =
                        (from np in entity.GetNavigationProperties(false)
                         where np.Property == member
                         select np.ValueIsNull).Any(isNull => isNull);

                    return !isEmptyNavProp;
                };
            return property;
        }
    }
    private class CollectionSerializationHelper<T>
    {
        [JsonProperty]
        public List<T> Result = [];
    }
}
