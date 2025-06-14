namespace MessageDecoder.NMEAObjects;

/// <summary>
/// Known NMEA sources for satnav messages
/// </summary>
/// <remarks>
/// Some devices may always use <see cref="GPS"/> even if the source is a different system
/// </remarks>
public enum SatnavSource
{
    /// <summary>
    /// Unknown or not yet standardized source
    /// </summary>
    Other,
    /// <summary>
    /// Satnav operated by the People's Republic of China
    /// <a href="http://en.beidou.gov.cn/">en.beidou.gov.cn</a>
    /// </summary>
    Beidou,
    /// <summary>
    /// Satnav operated by the European Union
    /// <a href="https://www.gsc-europa.eu/">gsc-europa.eu</a>
    /// </summary>
    Gallileo,
    /// <summary>
    /// Satnav operated by the United States of America
    /// <a href="https://www.gps.gov/">gps.gov</a>
    /// </summary>
    GPS,
    /// <summary>
    /// Satnav operated by the Russian Federation
    /// <a href="https://www.glonass-iac.ru/en/">glonass-iac.ru</a>
    /// </summary>
    GLONASS,
    /// <summary>
    /// Not a satnav at all. Used for a few outdated messages.
    /// Most receivers will pretend to be <see cref="GPS"/> instead
    /// </summary>
    Loran
}
