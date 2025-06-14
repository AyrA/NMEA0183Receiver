namespace MessageDecoder.Internals;

/// <summary>
/// Provides enum utilities
/// </summary>
internal static class EnumTools
{
    /// <summary>
    /// Gets the first enum value starting with the given substring
    /// (in NMEA, usually just the first letter)
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="value">String value to parse</param>
    /// <param name="fallback">
    /// Fallback if parsing fails.
    /// If this is null or not specified,
    /// the method will throw an exception if <paramref name="value"/> cannot be parsed
    /// </param>
    /// <returns>Parsed enum</returns>
    public static T ByFirstLetter<T>(string value, T? fallback) where T : struct, Enum
    {
        try
        {
            var name = Enum.GetNames<T>().First(m => m.StartsWith(value, StringComparison.InvariantCultureIgnoreCase));
            return Enum.Parse<T>(name);
        }
        catch
        {
            if (fallback != null)
            {
                return fallback.Value;
            }
            throw;
        }
    }

    /// <summary>
    /// Gets the first enum value starting with the given substring
    /// (in NMEA, usually just the first letter)
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="value">String value to parse</param>
    /// <returns>Parsed enum. Throws if unable to parse</returns>
    public static T ByFirstLetter<T>(string value) where T : struct, Enum
    {
        return ByFirstLetter<T>(value, null);
    }
}
