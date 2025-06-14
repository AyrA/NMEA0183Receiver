using MessageDecoder.Internals;
using MessageDecoder.NMEAObjects;

namespace MessageDecoder.Messages;

/// <summary>
/// Recommended minimum message (__RMC)
/// </summary>
/// <remarks>
/// This is the preferred message to plot onto a map.
/// Other message types should be ignored in most applications
/// </remarks>
public class RecommendedMinumumMessage : BaseSatnavMessage
{
    /// <summary>
    /// Date and time when the fix was taken
    /// </summary>
    /// <remarks>
    /// The year in the NMEA message is only two digits wide.
    /// In this implemtation it's assumed this value is post year 2000.<br />
    /// If the date is many years in the past (~19.6 years),
    /// your receiver may suffer from the week rollover problem.
    /// A GPS date is basically the seconds within the current week
    /// plus a number of weeks since 1980-01-06 00:00 UTC.
    /// There are only enough bits for weeks 0-1023 before it rolls over.
    /// While the GPS message format was updated to contain more week bits,
    /// old receivers may not have this in their firmware.
    /// If your date is multiple years off from what it should be,
    /// check if adding 7168 days to the date fixes the problem.<br />
    /// <b>
    /// Do not use this value to set your local clock,
    /// it will not be accurate enough when compared to other sources.
    /// </b>
    /// </remarks>
    public DateTime FixTaken { get; }

    /// <summary>
    /// Gets whether position dat is valid or not
    /// </summary>
    public FixStatus Status { get; }

    /// <summary>
    /// Positional latitude
    /// </summary>
    public PositionInfo Latitude { get; }

    /// <summary>
    /// Positional longitude
    /// </summary>
    public PositionInfo Longitude { get; }

    /// <summary>
    /// Speed in knots
    /// </summary>
    public decimal SpeedKnots { get; }

    /// <summary>
    /// Speed in meters per second
    /// </summary>
    public decimal SpeedMeters => SpeedKnots / 1.943844m;

    /// <summary>
    /// Compass heading
    /// </summary>
    public decimal TrackAngle { get; }

    /// <summary>
    /// Magnetic variation
    /// </summary>
    /// <remarks>
    /// This is the course correction necessary to track magnetic north instead of true north
    /// </remarks>
    public PositionInfo? MagneticVariation { get; }

    /// <summary>
    /// Source of data
    /// </summary>
    public FixSource DataSource { get; }

    internal RecommendedMinumumMessage(string line) : base(line)
    {
        FixTaken = LineUtils.ToDateTime(_parts[1], _parts[9]);
        Status = EnumTools.ByFirstLetter<FixStatus>(_parts[2], FixStatus.Void);
        Latitude = new(_parts[3], _parts[4]);
        Longitude = new(_parts[5], _parts[6]);
        if (_parts[7].HasContent())
        {
            SpeedKnots = decimal.Parse(_parts[7]);
        }
        if (_parts[8].HasContent())
        {
            TrackAngle = decimal.Parse(_parts[8]);
        }
        if (_parts[10].HasContent() && _parts[11].HasContent())
        {
            MagneticVariation = new(_parts[10], _parts[11]);
        }

        if (_parts.Length >= 13) //Added in version 2.3
        {
            DataSource = EnumTools.ByFirstLetter<FixSource>(_parts[12], FixSource.NotValid);
        }
        IsValid = Status != FixStatus.Void;
    }
}
