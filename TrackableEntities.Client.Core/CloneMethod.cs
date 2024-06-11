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
    Memberwise  //stack overflow
}

/// <summary>
/// Switches default cloning method.
/// </summary>
public static class CloneMethodSetting
{
    /// <summary>
    /// Default cloning method.
    /// </summary>
    public const CloneMethod Default = CloneMethod.SystemTextJsonSerialized;
}
