using MessageDecoder.Internals;

namespace MessageDecoder.NMEAObjects;

/// <summary>
/// Represents the position on the global coordinate system
/// </summary>
public class PositionInfo
{
    /// <summary>
    /// Degrees
    /// </summary>
    public int Degrees { get; }

    /// <summary>
    /// Minutes and seconds formatted as decimal
    /// </summary>
    public decimal Minutes { get; }

    /// <summary>
    /// Compass direction.
    /// This will be north/south for latitude,
    /// or east/west for longitude
    /// </summary>
    public Compass Compass { get; }

    public PositionInfo(string pos, string compass)
    {
        if (pos.HasContent() && compass.HasContent())
        {
            try
            {
                Compass = EnumTools.ByFirstLetter<Compass>(compass, Compass.Invalid);
                var dot = pos.IndexOf('.');
                if (dot < 0)
                {
                    throw new ArgumentException("Invalid position value");
                }
                Degrees = int.Parse(pos[..(dot - 2)].TrimStart('0'));
                Minutes = decimal.Parse(pos[(dot - 2)..]);
            }
            catch
            {
                Degrees = 0; Minutes = 0; Compass = Compass.Invalid;
            }
        }
        else
        {
            Compass = Compass.Invalid;
        }
    }

    public override string ToString()
    {
        if (Compass == Compass.Invalid)
        {
            return "Invalid";
        }
        return $"{Degrees}° {Minutes}' {Compass.ToString()[0]}";
    }
}
