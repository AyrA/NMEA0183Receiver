namespace MessageDecoder.Messages;

/// <summary>
/// Base class for all NMEA messages
/// </summary>
public abstract class BaseMessage
{
    protected readonly string[] _parts;

    /// <summary>
    /// Type of the message
    /// </summary>
    /// <remarks>This is the first part between the starting $ and the first comma</remarks>
    public string MessageType { get; }

    /// <summary>
    /// Gets whether a checksum was present or not
    /// </summary>
    /// <remarks>Checksums are optional, but almost always present</remarks>
    public bool ChecksumPresent { get; }

    /// <summary>
    /// Gets whether checksum validation passed
    /// </summary>
    /// <remarks>
    /// Always false if <see cref="ChecksumPresent"/> is false.
    /// The message should be discarded if this is false,
    /// but <see cref="ChecksumPresent" /> is true.
    /// </remarks>
    public bool ChecksumValid { get; }

    /// <summary>
    /// Gets the checksum byte
    /// </summary>
    /// <remarks>
    /// Always zero if <see cref="ChecksumPresent"/> is false
    /// </remarks>
    public byte Checksum { get; }

    /// <summary>
    /// Gets the raw message parts minus the starting "$" and the checksum
    /// </summary>
    public string[] Parts => (string[])_parts.Clone();

    /// <summary>
    /// Performs basic message parsing
    /// </summary>
    /// <param name="line">Raw NMEA line</param>
    public BaseMessage(string line)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(line);
        line = line.Trim();
        //Message must be printable ASCII only
        if (line.Any(m => m < 0x20 || m >= 0x7F))
        {
            throw new FormatException("Line must be printable ASCII only");
        }
        //Messages could start with "!" but this is only for messages that need special encapsulation
        if (!line.StartsWith('$'))
        {
            throw new FormatException("NMEA message must start with '$' but did not");
        }
        //Skip the initial "$" when splitting
        _parts = line[1..].Split(',');

        //The message type is always 5 characters in "XXYYY" form,
        //where XX is the data source and YYY the message name.
        //The BaseSatnavMessage class provides access to the XX part.
        MessageType = _parts[0];

        //Not all messages have a checksum
        if (LineUtils.ChecksumValidator().IsMatch(line))
        {
            //Strip checksum (last 3 characters) from last part
            _parts[^1] = _parts[^1][..^3];
            //Checksum always is two digit hex
            var checksum = int.Parse(line[^2..], System.Globalization.NumberStyles.HexNumber);
            Checksum = (byte)checksum;

            //Algorithm:
            //----------
            //The Checksum is simply the XOR of all bytes between (but excluding)
            //the starting '$' and the '*' before the checksum
            //
            //Shortcut:
            //---------
            //Because of how the checksum is calculated,
            //XOR(XOR(..bytes), checksum) is zero on valid checksums.
            //As alternative, we could declare an extra variable to calculate checksum,
            //then compare it against the checksum. Using the checksum itself and comparing
            //against zero is just as fast and saves us another value allocation
            foreach (var c in line[1..^3])
            {
                checksum ^= c;
            }
            ChecksumValid = checksum == 0;
            ChecksumPresent = true;
        }
        else
        {
            ChecksumPresent = ChecksumValid = false;
            Checksum = 0;
        }
    }
}
