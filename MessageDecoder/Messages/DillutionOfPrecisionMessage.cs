using MessageDecoder.Internals;
using MessageDecoder.NMEAObjects;

namespace MessageDecoder.Messages;

/// <summary>
/// Dillution of precision (__GSA)
/// </summary>
public class DillutionOfPrecisionMessage : BaseSatnavMessage
{
    private readonly int[] _satellites;

    /// <summary>
    /// Gets the satellite selection type
    /// </summary>
    public FixSelection FixSelection { get; }

    /// <summary>
    /// Gets the fix type
    /// </summary>
    /// <remarks>
    /// If this is <see cref="FixType.None"/> then any value in this instance should be discarded
    /// </remarks>
    public FixType FixType { get; }

    /// <summary>
    /// Gets the satellite ids used to obtain the values in this instance
    /// </summary>
    public int[] Satellites => (int[])_satellites.Clone();

    /// <summary>
    /// Dillution of the precision overall. 1.0 would be perfect accuracy
    /// </summary>
    public decimal? DillutionOfPrecision { get; }

    /// <summary>
    /// Dillution of the precision on the horizontal axis. 1.0 would be perfect accuracy
    /// </summary>
    public decimal? HorizontalDillution { get; }

    /// <summary>
    /// Dillution of the precision on the vertical axis. 1.0 would be perfect accuracy
    /// </summary>
    public decimal? VerticalDillution { get; }

    internal DillutionOfPrecisionMessage(string line) : base(line)
    {
        try
        {
            FixSelection = EnumTools.ByFirstLetter<FixSelection>(_parts[1]);
            FixType = (FixType)int.Parse(_parts[2]);
            if (!Enum.IsDefined(FixType))
            {
                throw new ArgumentException("Invalid fix type");
            }
            _satellites = [.. _parts.Skip(3).Take(12).Where(m => !string.IsNullOrWhiteSpace(m)).Select(int.Parse).Order()];
            if (_parts[15].HasContent())
            {
                DillutionOfPrecision = decimal.Parse(_parts[15]);
            }

            if (_parts[16].HasContent())
            {
                HorizontalDillution = decimal.Parse(_parts[16]);
            }

            if (_parts[17].HasContent())
            {
                VerticalDillution = decimal.Parse(_parts[17]);
            }
            IsValid = DillutionOfPrecision.HasValue;
        }
        catch
        {
            _satellites = [];
            //NOOP
        }
    }

    public override string ToString()
    {
        if (IsValid)
        {
            return $"Dillution: {DillutionOfPrecision}; Hor: {HorizontalDillution}; Vert: {VerticalDillution}";
        }
        return "Invalid";
    }
}
