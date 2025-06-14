namespace MessageDecoder.NMEAObjects;

/// <summary>
/// Position fix type
/// </summary>
public enum FixType
{
    /// <summary>
    /// Position cannot be fixed
    /// </summary>
    /// <remarks>
    /// This usually means there's not enough satellites available,
    /// or their signals don't make sense when combined together.
    /// Date and time may still be available in this scenario.
    /// </remarks>
    None = 1,
    /// <summary>
    /// (2D) Latitude and longitude fix
    /// </summary>
    TwoDimensional = 2,
    /// <summary>
    /// (3D) Latitude, longitude, and altitude fix
    /// </summary>
    ThreeDimensional = 3
}
