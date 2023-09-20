#nullable enable
using System;
[AttributeUsage(AttributeTargets.Class)]
[System.Diagnostics.Conditional("TrackableEntityGenerator_DEBUG")]
internal sealed class TrackableEntityAttribute : Attribute
{
    public string[]? UsingDirectives { get; }  
    public TrackableEntityAttribute(params string[] usingDirectives) 
        => this.UsingDirectives = usingDirectives;
}
[AttributeUsage(AttributeTargets.Property)]
[System.Diagnostics.Conditional("TrackableEntityGenerator_DEBUG")]
internal sealed class TrackableEntityTrackedPropertyAttribute : Attribute
{
    public bool IsManyToMany { get; }
    public TrackableEntityTrackedPropertyAttribute(bool isManyToMany = false)
        => this.IsManyToMany = isManyToMany;
}
[AttributeUsage(AttributeTargets.Property)]
[System.Diagnostics.Conditional("TrackableEntityGenerator_DEBUG")]
internal sealed class TrackableEntityPropertyIgnoreAttribute : Attribute
{
    public TrackableEntityPropertyIgnoreAttribute()
    {
    }
}
[AttributeUsage(AttributeTargets.Class)]
[System.Diagnostics.Conditional("TrackableEntityGenerator_DEBUG")]
internal sealed class TrackableEntityCopyAttribute : Attribute
{
    public string[]? UsingDirectives { get; }  
    public TrackableEntityCopyAttribute(params string[] usingDirectives)
        => this.UsingDirectives = usingDirectives;
}