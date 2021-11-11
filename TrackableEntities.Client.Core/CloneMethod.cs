namespace TrackableEntities.Client.Core
{
    public enum CloneMethod
    {
        JsonSerialized,
        Memberwise
    }


    public static class CloneMethodSetting
    {
        public const CloneMethod Default = CloneMethod.JsonSerialized;
    }
}
