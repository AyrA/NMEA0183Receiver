namespace MessageDecoder.NMEAObjects;

/// <summary>
/// Gets the fix status
/// </summary>
public enum FixStatus
{
    /// <summary>
    /// Data source is available and of sufficient quality to derive values from
    /// </summary>
    Active,
    /// <summary>
    /// Data source is unavailable, unreliable, or there are not enough satellites available
    /// for reliable value derivation
    /// </summary>
    /// <remarks>Messages with this value should be discarded</remarks>
    Void
}
