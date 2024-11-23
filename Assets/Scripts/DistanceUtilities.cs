public static class DistanceUtilities
{
    /// <summary>
    /// Converts the unity distance values to a nautical miles
    /// </summary>
    public static float NauticalMilesToUnityUnit(float distance)
    {
        return distance * 100f;
    }
}