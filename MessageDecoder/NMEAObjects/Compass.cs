namespace MessageDecoder.NMEAObjects;

/// <summary>
/// Compass heading
/// </summary>
/// <remarks>
/// This is used for latitude and longitude.
/// Direction is usually given in a 360° heading from north
/// </remarks>
public enum Compass
{
    /// <summary>
    /// Invalid or unknown value
    /// </summary>
    Invalid = 0,
    /// <summary>
    /// North
    /// </summary>
    North = 1,
    /// <summary>
    /// East
    /// </summary>
    East = 2,
    /// <summary>
    /// South
    /// </summary>
    South = 3,
    /// <summary>
    /// West
    /// </summary>
    West = 4
}
