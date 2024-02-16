namespace TrackableEntities.IncrementalGenerator;

public class ClientEntityToGenerate(string className, bool modelOverride, IEnumerable<ClientEntityProperty> properties)
{
    public string ClassName { get; } = className;
    public bool ModelOverride { get; } = modelOverride;
    public IEnumerable<ClientEntityProperty> Properties { get; } = properties;
}

public class ClientEntityProperty(string name, string baseType, bool nullable, string? initializer, bool collection, bool tracked, bool setter, bool useNewtonsoftJson, bool useSystemTextJson, bool jsonIgnored, bool manyToMany)
{
    public string Name { get; } = name;
    public string BaseType { get; } = baseType;
    public bool Nullable { get; } = nullable;
    public string? Initializer { get; } = initializer;
    public bool Collection { get; } = collection;
    public bool Tracked { get; } = tracked;
    public bool UseNewtonsoftJson { get; } = useNewtonsoftJson;
    public bool UseSystemTextJson { get; } = useSystemTextJson;
    public bool JsonIgnored { get; } = jsonIgnored;
    public bool Setter { get; } = setter;
    public bool ManyToMany { get; } = manyToMany;
}