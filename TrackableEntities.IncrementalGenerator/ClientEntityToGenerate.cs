namespace TrackableEntities.IncrementalGenerator;

public class ClientEntityToGenerate
{
    public string ClassName { get; }
    public bool ModelOverride { get; }
    public IEnumerable<ClientEntityProperty> Properties { get; }
    public ClientEntityToGenerate(string className, bool modelOverride, IEnumerable<ClientEntityProperty> properties)
    {
        ClassName = className;
        ModelOverride = modelOverride;
        Properties = properties;
    }
}

public class ClientEntityProperty
{
    public string Name { get; }
    public string BaseType { get; }
    public bool Nullable { get; }
    public string? Initializer { get; }
    public bool Collection { get; }
    public bool Tracked { get; }    
    public bool PostUpdate { get; }
    public bool AllowJsonIgnore { get; }
    public bool JsonIgnored { get; }
    public bool Setter { get; }
    public ClientEntityProperty(string name, string baseType, bool nullable, string? initializer, bool collection, bool tracked, bool setter, bool postUpdate, bool allowJsonIgnore, bool jsonIgnored)
    {
        Name = name;
        BaseType = baseType;
        Nullable = nullable;
        Initializer = initializer;
        Collection = collection;
        Tracked = tracked;
        Setter = setter;
        PostUpdate = postUpdate;
        AllowJsonIgnore = allowJsonIgnore;
        JsonIgnored = jsonIgnored;
    }
}