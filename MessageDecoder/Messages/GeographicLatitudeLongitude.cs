using MessageDecoder.Internals;
using MessageDecoder.NMEAObjects;

namespace MessageDecoder.Messages;

/// <summary>
/// Geographical latitude message (__GLL)
/// </summary>
/// <remarks>
/// This is an outdated message and should not be used
/// in favor of __RMC messages
/// </remarks>
public class GeographicLatitudeLongitude : BaseSatnavMessage
{
    /// <summary>
    /// Time the fix was taken
    /// </summary>
    public TimeOnly FixTaken { get; }

    /// <summary>
    /// Validity of the fix
    /// </summary>
    public FixStatus Status { get; }

    /// <summary>
    /// Latitude position
    /// </summary>
    public PositionInfo? Latitude { get; }

    /// <summary>
    /// Longitude position
    /// </summary>
    public PositionInfo? Longitude { get; }

    /// <summary>
    /// Source for the data
    /// </summary>
    public FixSource DataSource { get; }

    internal GeographicLatitudeLongitude(string line) : base(line)
    {
        if (_parts[1].HasContent() && _parts[2].HasContent())
        {
            Latitude = new(_parts[1], _parts[2]);
        }
        if (_parts[3].HasContent() && _parts[4].HasContent())
        {
            Longitude = new(_parts[3], _parts[4]);
        }

        if (_parts[5].HasContent())
        {
            FixTaken = LineUtils.ToTime(_parts[5]);
        }
        Status = EnumTools.ByFirstLetter<FixStatus>(_parts[6], FixStatus.Void);
        if (_parts.Length >= 8 && _parts[7].HasContent()) //Added in version 2.3
        {
            DataSource = EnumTools.ByFirstLetter<FixSource>(_parts[7], FixSource.NotValid);
        }
        IsValid = Status != FixStatus.Void;
    }

    public override string ToString()
    {
        if (!IsValid)
        {
            return "Invalid";
        }
        return $"Lat: {Latitude}; Long: {Longitude}";
    }
}