using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using TrackableEntities.Common.Core;
namespace TrackableEntities.Client.Core;

internal class CloneLibrarySystemTextJson
{
    /// <summary>
    /// Create a deep clone of an object using System.Text.Json.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    public static T Clone<T>(T item) where T : class, ITrackable
    {
        return CloneObject(item);
    }

    /// <summary>
    /// Create a deep clone of a collection using System.Text.Json.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    public static IEnumerable<T> Clone<T>(IEnumerable<T> items)
    {
        return CloneObject(new CollectionSerializationHelper<T>() { Result = items.ToList() })?.Result ?? Enumerable.Empty<T>();
    }

    internal static T CloneObject<T>(T item) where T : class
    {
        var textJsonSettings = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { EntityNavigationPropertyModifier } }
        };
        return JsonSerializer.Deserialize<T?>(JsonSerializer.Serialize(item, textJsonSettings), textJsonSettings) ?? throw new InvalidCastException();
    }
    private static void EntityNavigationPropertyModifier(JsonTypeInfo typeInfo)
    {
        foreach (JsonPropertyInfo property in typeInfo.Properties)
        {
            property.ShouldSerialize = (obj, value) =>
            {
                if (obj is not ITrackable entity) return true;

                // The current property is a navigation property and its value is null
                bool isEmptyNavProp =
                    (from np in entity.GetNavigationProperties(false)
                     where np.Property == obj.GetType().GetProperty(property.Name)
                     select np.ValueIsNull).Any(isNull => isNull);

                return !isEmptyNavProp;
            };
        }
    }

    private CloneChangeHelper _cloneChangeHelper = new();
    private static readonly JsonSerializerOptions ChangeTrackingDeserializerOptions = new() { ReferenceHandler = ReferenceHandler.Preserve };


    /// <summary>
    /// Create a deep clone of any changes to an ITrackable object using System.Text.Json.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public T CloneChanges<T>(T item) where T : class, ITrackable
    {   
        _cloneChangeHelper = new();
        // Inspect the graph and collect entityChangedInfos
        _ = _cloneChangeHelper.GetChanges([item]).ToList();
#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances. We can't cache as we're loading the ReferenceHandler with the supplied CloneChangeHelper.
        var serializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = new CloneChangeReferenceHandler(_cloneChangeHelper),  //allow keeping track of reference Ids, CloneChangeHelper across entire serialization
            Converters = { new ChangeTrackingCollectionChangesConverter() }, //refer to _cloneChangeHelper to see if collection items should be serialized
            TypeInfoResolver = new DefaultJsonTypeInfoResolver() { Modifiers = { GetChangedPropertyModifier } } //refer to _cloneChangeHelper to see if properties should be serialized
        };        
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances
        return JsonSerializer.Deserialize<T?>(JsonSerializer.Serialize(item, serializerOptions), ChangeTrackingDeserializerOptions) ?? throw new InvalidCastException();
    }

    private void GetChangedPropertyModifier(JsonTypeInfo typeInfo)
    {
        foreach (JsonPropertyInfo property in typeInfo.Properties)
        {
            property.ShouldSerialize = (obj, value) =>
            {
                if (obj is not ITrackable entity || value is null) return true;

                EntityNavigationProperty? np = entity.GetNavigationProperties(false).FirstOrDefault(x => x.Property == obj.GetType().GetProperty(property.Name));
                if (np == null) return true; // not a nav prop
                if (np.ValueIsNull) return false; // nav prop is not initialized
                foreach (var rp in np.AsReferenceProperty())
                {
                    if (rp.Property is null) continue;
                    // don't serialize unchanged reference navigation props
                    return _cloneChangeHelper.IncludeReferenceProp(entity, rp.Property);
                }                

                // inspect collection navigation props, don't serialize property if there are no changes to the collection
                foreach (var cp in np.AsCollectionProperty())
                {
                    if (cp.Property is null || cp.EntityCollection is null) continue;
                    foreach (var val in cp.EntityCollection)
                    {
                        if (_cloneChangeHelper.IncludeCollectionItem(entity, cp.Property, val))
                            return true;
                    }
                }
                //no changes in this nav prop, don't serialize the property
                return false;
            };
        }
    }

    private class ChangeTrackingCollectionChangesConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type type)
        {
            if (!type.IsGenericType) return false;
            var args = type.GetGenericArguments();
            //generic type that has a single ITrackable argument
            if (args.Length != 1 || !args[0].GetInterfaces().Any(i => i == typeof(ITrackable))) return false;
            //and implements IEnumerable
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));            
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type elementType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(JsonCollectionChangedItemsConverter<>).MakeGenericType(elementType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

    }
    /// <summary>
    /// Json changes-only collection converter. Serializes only the collection items as specified in CloneChangeHelper.
    /// </summary>
    /// <typeparam name="TEntity">Type of item to convert.</typeparam>  
    private class JsonCollectionChangedItemsConverter<TEntity> : JsonConverter<IEnumerable<TEntity>>        
    {
        private readonly static JsonConverter<TEntity> s_defaultConverter = (JsonConverter<TEntity>)JsonSerializerOptions.Default.GetConverter(typeof(TEntity));        
        /// <summary>
        /// Reads a json string and deserializes it into an object.
        /// </summary>
        /// <param name="reader">Json reader.</param>
        /// <param name="typeToConvert">Type to convert.</param>
        /// <param name="options">Serializer options.</param>
        /// <returns>Created object.</returns>
        public override IEnumerable<TEntity> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a json string for elements in the collection that have changes.
        /// </summary>
        /// <param name="writer">Json writer.</param>
        /// <param name="value">Value to write.</param>
        /// <param name="options">Serializer options.</param>
        public override void Write(Utf8JsonWriter writer, IEnumerable<TEntity> value, JsonSerializerOptions options)
        {
            var cloneChangeHelper = ((CloneChangeReferenceHandler)options.ReferenceHandler!).CloneChangeHelper;
            writer.WriteStartArray();
            foreach (var entity in value.Cast<ITrackable>())
            {
                if (entity.TrackingState != TrackingState.Unchanged) goto Serialize;  //serialize any changed entity
                var navProperties = entity.GetNavigationProperties(false).ToList();
                if (navProperties.Count == 0 && entity.TrackingState == TrackingState.Unchanged) continue;
                foreach (var np in navProperties)
                {
                    foreach (var rp in np.AsReferenceProperty())
                    {
                        if (rp.Property is null || rp.ValueIsNull) continue;
                        // serialize changed reference navigation props
                        if (cloneChangeHelper.IncludeReferenceProp(entity, rp.Property))
                            goto Serialize;
                    }

                    // inspect collection navigation props, serialize entity if its collection has changes
                    foreach (var cp in np.AsCollectionProperty())
                    {
                        if (cp.Property is null || cp.EntityCollection is null) continue;
                        foreach (var val in cp.EntityCollection)
                        {
                            if (cloneChangeHelper.IncludeCollectionItem(entity, cp.Property, val))
                                goto Serialize;
                        }
                    }
                }
                continue;
            Serialize:
                s_defaultConverter.Write(writer, (TEntity)entity, options);
            }
            
            writer.WriteEndArray();
        }
    }

    class CloneChangeReferenceHandler(CloneChangeHelper helper) : ReferenceHandler
    {
        private readonly ReferenceResolver _rootedResolver = new CloneChangeReferenceResolver();
        public readonly CloneChangeHelper CloneChangeHelper = helper;
        public override ReferenceResolver CreateResolver() => _rootedResolver;

        private class CloneChangeReferenceResolver : ReferenceResolver
        {
            private uint _referenceCount;
            private readonly Dictionary<string, object> _referenceIdToObjectMap = [];
            private readonly Dictionary<object, string> _objectToReferenceIdMap = new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

            public override void AddReference(string referenceId, object value)
            {
                if (!_referenceIdToObjectMap.TryAdd(referenceId, value))
                {
                    throw new JsonException();
                }
            }

            public override string GetReference(object value, out bool alreadyExists)
            {
                if (_objectToReferenceIdMap.TryGetValue(value, out string? referenceId))
                {
                    alreadyExists = true;
                }
                else
                {
                    _referenceCount++;
                    referenceId = _referenceCount.ToString();
                    _objectToReferenceIdMap.Add(value, referenceId);
                    alreadyExists = false;
                }

                return referenceId;
            }

            public override object ResolveReference(string referenceId)
            {
                if (!_referenceIdToObjectMap.TryGetValue(referenceId, out object? value))
                {
                    throw new JsonException();
                }

                return value;
            }
        }
    }


    private class CollectionSerializationHelper<T>
    {
        [JsonInclude]
        public List<T> Result = [];
    }
}
