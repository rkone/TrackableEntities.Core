using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TrackableEntities.Client.Core;

//when using System.Text.Json, this class will allow sending more data to the client, and the client will return only tracked data.
//On the client, add the OverrideJsonIgnoreContractResolver to the JsonSerializerOptions:
//ie: JsonSerializerOptions DefaultJsonSerializerOptions = new() { ReferenceHandler = ReferenceHandler.Preserve, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault, TypeInfoResolver = new OverrideJsonIgnoreContractResolver() };
/// <summary>
/// JsonTypeInfoResolver that allows properties with the JsonIgnore attribute to be deserialized.
/// </summary>
public class OverrideJsonIgnoreContractResolver : DefaultJsonTypeInfoResolver
{
    /// <summary>
    /// Overrides the type's property setters that have the JsonIgnore attribute
    /// </summary>
    /// <param name="type"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var typeInfo = base.GetTypeInfo(type, options);
        if (!type.IsSubclassOf(typeof(EntityBase))) return typeInfo;
        if (typeInfo.Kind == JsonTypeInfoKind.Object)
        {
            foreach (var property in type.GetProperties())
            {
                if (property.GetCustomAttribute<JsonIgnoreAttribute>() is null) continue;

                var jsonProp = typeInfo.Properties.FirstOrDefault(p => p.Name == property.Name);
                if (jsonProp is null) continue;

                jsonProp.Set = JsonExtensions.CreateSetter(typeInfo.Type, property.GetSetMethod(true));
            }
        }
        return typeInfo;
    }
}

/// <summary>
/// Creation of default getters and setters for properties
/// </summary>
// https://stackoverflow.com/questions/61869393/get-net-core-jsonserializer-to-serialize-private-members
public static class JsonExtensions
{
    delegate TValue RefFunc<TObject, TValue>(ref TObject arg);
    /// <summary>
    /// Creates a default getter for a property
    /// </summary>
    /// <param name="type"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static Func<object, object?>? CreateGetter(Type type, MethodInfo? method)
    {
        if (method == null)
            return null;
        var myMethod = typeof(JsonExtensions).GetMethod(nameof(CreateGetterGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;
        return (Func<object, object?>)myMethod.MakeGenericMethod([type, method.ReturnType]).Invoke(null, new[] { method })!;
    }

    static Func<object, object?> CreateGetterGeneric<TObject, TValue>(MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);
        if (typeof(TObject).IsValueType)
        {
            // https://stackoverflow.com/questions/4326736/how-can-i-create-an-open-delegate-from-a-structs-instance-method
            // https://stackoverflow.com/questions/1212346/uncurrying-an-instance-method-in-net/1212396#1212396
            var func = (RefFunc<TObject, TValue>)Delegate.CreateDelegate(typeof(RefFunc<TObject, TValue>), null, method);
            return (o) => { var tObj = (TObject)o; return func(ref tObj); };
        }
        else
        {
            var func = (Func<TObject, TValue>)Delegate.CreateDelegate(typeof(Func<TObject, TValue>), method);
            return (o) => func((TObject)o);
        }
    }
    /// <summary>
    /// Creates a default setter for a property
    /// </summary>
    /// <param name="type"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static Action<object, object?>? CreateSetter(Type type, MethodInfo? method)
    {
        if (method == null)
            return null;
        var myMethod = typeof(JsonExtensions).GetMethod(nameof(CreateSetterGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;
        return (Action<object, object?>)myMethod.MakeGenericMethod([type, method.GetParameters().Single().ParameterType]).Invoke(null, new[] { method })!;
    }

    static Action<object, object?>? CreateSetterGeneric<TObject, TValue>(MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);
        if (typeof(TObject).IsValueType)
        {
            // TODO: find a performant way to do this.  Possibilities:
            // Box<T> from Microsoft.Toolkit.HighPerformance
            // https://stackoverflow.com/questions/18937935/how-to-mutate-a-boxed-struct-using-il
            return (o, v) => method.Invoke(o, [v]);
        }
        else
        {
            var func = (Action<TObject, TValue?>)Delegate.CreateDelegate(typeof(Action<TObject, TValue?>), method);
            return (o, v) => func((TObject)o, (TValue?)v);
        }
    }
    static IEnumerable<Type> BaseTypesAndSelf(this Type? type)
    {
        while (type != null)
        {
            yield return type;
            type = type.BaseType;
        }
    }
}
