using MessageDecoder.Internals;

namespace MessageDecoder.NMEAObjects;

/// <summary>
/// Altitude information
/// </summary>
public class AltitudeInfo
{
    /// <summary>
    /// Altitude value
    /// </summary>
    public decimal Altitude { get; }

    /// <summary>
    /// Altitude unit
    /// </summary>
    public AltitudeUnit Unit { get; }

    public AltitudeInfo(string height, string unit)
    {
        Unit = EnumTools.ByFirstLetter<AltitudeUnit>(unit, AltitudeUnit.Invalid);
        if (decimal.TryParse(height, out var value))
        {
            Altitude = value;
        }
        else
        {
            Unit = AltitudeUnit.Invalid;
        }
    }

    public override string ToString()
    {
        return Unit == AltitudeUnit.Invalid ? "Invalid" : $"{Altitude} {Unit}";
    }
}
