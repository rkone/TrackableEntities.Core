namespace TrackableEntities.Client.Core;

public enum CloneMethod
{
    SystemTextJsonSerialized,
    NewtonsoftJsonSerialized,
    Memberwise  //Has issue with multiple uses of an entity in a graph.
                //IE adding Customer with Customer->Address, Customer->Contact->Address where Address is the same.
}


public static class CloneMethodSetting
{
    public const CloneMethod Default = CloneMethod.NewtonsoftJsonSerialized;
}
