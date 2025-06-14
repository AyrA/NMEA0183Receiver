using MessageDecoder.NMEAObjects;

namespace MessageDecoder.Messages;

/// <summary>
/// Global Positioning System Fix Data (__GGA)
/// </summary>
public class PositionMessage : BaseSatnavMessage
{
    /// <summary>
    /// Time the message was obtained
    /// </summary>
    public TimeOnly MessageTime { get; }

    /// <summary>
    /// Position latitude
    /// </summary>
    public PositionInfo Latitude { get; }

    /// <summary>
    /// Position longitude
    /// </summary>
    public PositionInfo Longitude { get; }

    /// <summary>
    /// Data quality
    /// </summary>
    public FixQuality FixQuality { get; }

    /// <summary>
    /// Number of satellites currently in view
    /// </summary>
    /// <remarks>Not necessarily all of them may be in use</remarks>
    public int NumberOfSatellites { get; }

    /// <summary>
    /// Dillution on the horizontal axis. 1.0 would be perfect accuracy.
    /// </summary>
    public decimal? HorizontalDilution { get; }

    /// <summary>
    /// Altitude above ground
    /// </summary>
    /// <remarks>
    /// This is given in the way it would read on a height map and usually doesn't needs any processing.
    /// To revert this to the height on a perfect sphere,
    /// see <see cref="HeightOfGeoid"/>
    /// </remarks>
    public AltitudeInfo? Altitude { get; }

    /// <summary>
    /// Height of geoid above ground in respect to a perfect sphere
    /// </summary>
    /// <remarks>
    /// Due to the rotation, earth is not perfectly round.
    /// This value specifies how much the sea level diverges from a perfect sphere.
    /// If altitude is present, this should also be present,
    /// otherwise the altitude is likely wrong
    /// </remarks>
    public AltitudeInfo? HeightOfGeoid { get; }

    /// <summary>
    /// Last update from a DGPS station in seconds, often zero or not available
    /// </summary>
    public int? LastDGPSUpdate { get; }

    /// <summary>
    /// DGPS station number, often zero or not available
    /// </summary>
    public int? DGPSStationNumber { get; }

    internal PositionMessage(string line) : base(line)
    {
        if (_parts.Length < 15)
        {
            throw new ArgumentException("GPS message must consist of at least 15 elements");
        }
        MessageTime = LineUtils.ToTime(_parts[1]);
        Latitude = new(_parts[2], _parts[3]);
        Longitude = new(_parts[4], _parts[5]);
        FixQuality = (FixQuality)int.Parse(_parts[6]);
        if (!Enum.IsDefined(FixQuality))
        {
            FixQuality = FixQuality.Invalid;
        }
        if (_parts[7].HasContent())
        {
            NumberOfSatellites = int.Parse(_parts[7].TrimStart('0').WhenEmpty("0"));
        }
        else
        {
            NumberOfSatellites = 0;
        }

        if (_parts[8].HasContent())
        {
            HorizontalDilution = decimal.Parse(_parts[8]);
        }

        if (_parts[9].HasContent() && _parts[10].HasContent())
        {
            Altitude = new(_parts[9], _parts[10]);
        }

        if (_parts[11].HasContent() && _parts[12].HasContent())
        {
            HeightOfGeoid = new(_parts[11], _parts[12]);
        }

        if (_parts[13].HasContent())
        {
            LastDGPSUpdate = int.Parse(_parts[13]);
        }

        if (_parts[14].HasContent())
        {
            DGPSStationNumber = int.Parse(_parts[14]);
        }

        IsValid = FixQuality != FixQuality.Invalid;
    }
}
