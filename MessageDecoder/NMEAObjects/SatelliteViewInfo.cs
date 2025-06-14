namespace MessageDecoder.NMEAObjects;

/// <summary>
/// Satellite info from <see cref="Messages.SatelliteViewMessage"/>
/// </summary>
public class SatelliteViewInfo
{
    /// <summary>
    /// RPN number of the satellite.
    /// This number should not change.
    /// The same number means the same satellite
    /// </summary>
    public int RpnNumber { get; }

    /// <summary>
    /// Elevation in degrees
    /// </summary>
    public int Elevation { get; }

    /// <summary>
    /// Azimuth in degrees
    /// </summary>
    public int Azimuth { get; }

    /// <summary>
    /// Signal to noise ratio
    /// </summary>
    /// <remarks>
    /// Higher is better.
    /// Zero indicates that the value cannot be estimated,
    /// or that the satellite is in view but is not in use by the device
    /// </remarks>
    public int SNR { get; }

    internal SatelliteViewInfo(string rpn, string elevation, string azimuth, string snr)
    {
        RpnNumber = int.Parse(rpn);
        if (elevation.HasContent())
        {
            Elevation = int.Parse(elevation);
        }
        if (azimuth.HasContent())
        {
            Azimuth = int.Parse(azimuth);
        }
        if (snr.HasContent())
        {
            SNR = Math.Clamp(int.Parse(snr), 0, 99);
        }
    }

    public override string ToString()
    {
        return $"RPN={RpnNumber}; EL={Elevation}; AZ={Azimuth}; SNR={SNR}";
    }
}
