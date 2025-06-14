using MessageDecoder.Messages;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MessageDecoder;

/// <summary>
/// Provides utilities to process parts of an NMEA line
/// </summary>
internal static partial class LineUtils
{
    private record MessageClass(string RegexMatch, Func<string, BaseMessage> Factory);

    /// <summary>
    /// Maps regex of the message type to the correct message
    /// </summary>
    /// <remarks>
    /// The regex must match the full message type string (excluding the initial '$')
    /// </remarks>
    private static readonly List<MessageClass> types =
    [
        new("..GGA", (s) => new PositionMessage(s)),
        new("..GSA", (s) => new DillutionOfPrecisionMessage(s)),
        new("..GSV", (s) => new SatelliteViewMessage(s)),
        new("..RMC", (s) => new RecommendedMinumumMessage(s)),
        new("..GLL", (s) => new GeographicLatitudeLongitude(s))
    ];

    /// <summary>
    /// Parses a single NMEA line into a message object
    /// </summary>
    /// <param name="line">Line to parse</param>
    /// <returns>Parsed instance, or <see cref="RawMessage"/> if none matched</returns>
    public static BaseMessage Parse(string line)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(line);
        if (!line.StartsWith('$'))
        {
            throw new ArgumentException("NMEA Line must start with '$'");
        }
        line = line[1..]; //Strip '$'

        var comma = line.IndexOf(',');
        var messageType = comma < 0 ? line : line[..comma];
        foreach (var item in types)
        {
            if (Regex.IsMatch(messageType, $"^{item.RegexMatch}$"))
            {
                return item.Factory(line);
            }
        }
        return new RawMessage(line);
    }

    /// <summary>
    /// Returns <paramref name="replacement"/> if <paramref name="source"/>
    /// is null, empty, or whitespace.
    /// Otherwise returns <paramref name="source"/>
    /// </summary>
    /// <param name="source">Potentially empty string</param>
    /// <param name="replacement">Substitution value</param>
    /// <returns>
    /// <paramref name="replacement"/> if <paramref name="source"/>
    /// is null, empty, or whitespace.
    /// Otherwise returns <paramref name="source"/>
    /// </returns>
    public static string WhenEmpty(this string source, string replacement)
    {
        return string.IsNullOrWhiteSpace(source) ? replacement : source;
    }

    /// <summary>
    /// Checks if a string is considered to have meaningful content
    /// </summary>
    /// <param name="s">String</param>
    /// <returns>true, if the string has content</returns>
    /// <remarks>Currently, this maps to <see cref="string.IsNullOrWhiteSpace(string?)"/></remarks>
    public static bool HasContent(this string s) => !string.IsNullOrWhiteSpace(s);

    /// <summary>
    /// Parses date and time components into a DateTime instance
    /// </summary>
    /// <param name="timePortion">NMEA time value</param>
    /// <param name="datePortion">NMEA date value</param>
    /// <returns>processed date and time</returns>
    /// <exception cref="ArgumentException">At least one value is invalid</exception>
    public static DateTime ToDateTime(string timePortion, string datePortion)
    {
        if (datePortion.Length != 6 || !int.TryParse(datePortion, out var parsed))
        {
            throw new ArgumentException($"Date portion of message is invalid: '{datePortion}'");
        }
        var year = (parsed % 100) + 2000;
        parsed /= 100;
        var month = parsed % 100;
        parsed /= 100;
        var day = parsed;

        var date = new DateOnly(year, month, day);
        var time = ToTime(timePortion);
        return new DateTime(date, time, DateTimeKind.Utc);
    }

    /// <summary>
    /// Parses a time string into a time object
    /// </summary>
    /// <param name="value">NMEA time string</param>
    /// <returns>Time object</returns>
    public static TimeOnly ToTime(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var parts = value.Split('.');
        var baseTime = BaseLineToTime(parts[0]);
        if (parts.Length > 1)
        {
            var millis = TimeSpan.FromSeconds(double.Parse("0." + parts[1]));
            baseTime = baseTime.Add(millis);
        }
        return baseTime;
    }

    /// <summary>
    /// Parse string into TimeOnly
    /// </summary>
    /// <param name="value">NMEA time value</param>
    /// <returns>Parsed time</returns>
    private static TimeOnly BaseLineToTime(string value)
    {
        return value.Length switch
        {
            4 => TimeOnly.ParseExact(value, "HHmm", CultureInfo.InvariantCulture),
            6 => TimeOnly.ParseExact(value, "HHmmss", CultureInfo.InvariantCulture),
            _ => throw new ArgumentException($"Invalid time value: '{value}'")
        };
    }

    [GeneratedRegex(@"\*[\dA-F]{2}\z", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    public static partial Regex ChecksumValidator();
}
