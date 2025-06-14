namespace MessageDecoder.NMEAObjects;

/// <summary>
/// Source of data fix message
/// </summary>
public enum FixSource
{
    /// <summary>
    /// Source operates fully autonomous
    /// </summary>
    Autonomous,
    /// <summary>
    /// Data is the result of multiple sources
    /// </summary>
    Differential,
    /// <summary>
    /// Data is estimated (possible dead reckoning)
    /// </summary>
    Estimated,
    /// <summary>
    /// Data is invalid
    /// </summary>
    /// <remarks>
    /// If this value is present, the message should be discarded
    /// </remarks>
    NotValid,
    /// <summary>
    /// Data is provided by a data simulator
    /// </summary>
    Simulator
}
