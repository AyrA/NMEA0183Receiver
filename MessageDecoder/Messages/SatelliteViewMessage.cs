using MessageDecoder.NMEAObjects;

namespace MessageDecoder.Messages;

/// <summary>
/// Message containing satellite information (__GSV)
/// </summary>
public class SatelliteViewMessage : BaseSatnavMessage
{
    private readonly List<SatelliteViewInfo> _satellites = [];

    /// <summary>
    /// Number of lines needed for the complete set of information
    /// </summary>
    public int NumberOfSentences { get; }

    /// <summary>
    /// Current line. Must be between 1 and <see cref="NumberOfSentences"/> inclusive
    /// </summary>
    public int Sentence { get; }

    /// <summary>
    /// Gets whether this item completes the message
    /// </summary>
    public bool IsComplete => NumberOfSentences == Sentence;

    /// <summary>
    /// Gets the number of satellites in view
    /// </summary>
    /// <remarks>
    /// This number may be less than the actual number of satellites in view.
    /// This is the number of satellites in view that are currently being tracked.
    /// </remarks>
    public int SatellitesInView { get; }

    /// <summary>
    /// Gets the satellite information contained in this message
    /// </summary>
    public SatelliteViewInfo[] Satellites => [.. _satellites];

    public SatelliteViewMessage(string line) : base(line)
    {
        NumberOfSentences = int.Parse(_parts[1]);
        Sentence = int.Parse(_parts[2]);
        SatellitesInView = int.Parse(_parts[3]);

        //4 satellites fit into a message,
        //but if less than 4 are remaining the message will not be padded with empty fields,
        //instead it is shortened
        var remaining = Math.Min(4, SatellitesInView - ((Sentence - 1) * 4));
        for (int i = 0; i < remaining; i++)
        {
            var offset = (i * 4) + 4;
            if (_parts[offset].HasContent())
            {
                _satellites.Add(new(_parts[offset], _parts[offset + 1], _parts[offset + 2], _parts[offset + 3]));
            }
        }
        IsValid = true;
    }
}
