namespace MessageDecoder.NMEAObjects;

/// <summary>
/// Quality of the source
/// </summary>
public enum FixQuality
{
    /// <summary>
    /// Source is invalid or not known
    /// </summary>
    Invalid = 0,
    /// <summary>
    /// SatNav device
    /// </summary>
    /// <remarks>
    /// Regardless of the name, this includes other systems too.
    /// See <see cref="SatnavSource"/> for a list of known NMEA sources
    /// </remarks>
    GPS = 1,
    /// <summary>
    /// Differential GPS fix
    /// </summary>
    /// <remarks>
    /// This is SatNav data, but with extra data from ground stations
    /// to increase accuracy.
    /// </remarks>
    DGPS = 2,
    /// <summary>
    /// GPS fix with extra post processing
    /// </summary>
    /// <remarks>
    /// Usually this means there are multiple data sources aggregated together
    /// </remarks>
    PPS = 3,
    /// <summary>
    /// Real time kinematic device
    /// </summary>
    RealTimeKinematic = 4,
    /// <summary>
    /// Real time kinematic device
    /// </summary>
    FloatRealTimeKinematic = 5,
    /// <summary>
    /// Estimated (dead reckoning)
    /// </summary>
    Estimated = 6,
    /// <summary>
    /// Manually provided data
    /// </summary>
    Manual = 7,
    /// <summary>
    /// Data provided by simulation
    /// </summary>
    Simulated = 8
}
