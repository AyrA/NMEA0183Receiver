using MessageDecoder.NMEAObjects;

namespace MessageDecoder.Messages;

/// <summary>
/// Base class for all satnav messages
/// </summary>
public abstract class BaseSatnavMessage(string line) : BaseMessage(line)
{
    /// <summary>
    /// Gets the data source of the message
    /// </summary>
    public SatnavSource Source
    {
        get
        {
            return _parts[0][..2].ToUpper() switch
            {
                "BD" or "GB" => SatnavSource.Beidou,
                "GA" => SatnavSource.Gallileo,
                "GP" => SatnavSource.GPS,
                "GL" => SatnavSource.GLONASS,
                "LC" => SatnavSource.Loran,
                _ => SatnavSource.Other,
            };
        }
    }

    /// <summary>
    /// Gets whether the message is valid or not
    /// </summary>
    /// <remarks>
    /// Invalid messages should be discarded
    /// because they're incomplete, or the data might be wrong
    /// </remarks>
    public bool IsValid { get; protected set; }
}
