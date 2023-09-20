namespace TrackableEntities.Client.Core;

/// <summary>
/// Cloning method types. NewtonSoft is currently the only one that passes all tests.
/// </summary>
public enum CloneMethod
{
    /// <summary>System.Jext.Json</summary>
    SystemTextJsonSerialized,
    /// <summary>Newtonsoft.Json</summary>
    NewtonsoftJsonSerialized,
    /// <summary>System.Runtime.Serialization</summary>
    Memberwise  //Has issue with multiple uses of an entity in a graph.
                //IE adding Customer with Customer->Address, Customer->Contact->Address where Address is the same.
}

/// <summary>
/// Switches default cloning method.
/// </summary>
public static class CloneMethodSetting
{
    /// <summary>
    /// Default cloning method.
    /// </summary>
    public const CloneMethod Default = CloneMethod.NewtonsoftJsonSerialized;
}
