namespace MessageDecoder.Messages;

/// <summary>
/// Represents a raw, unprocessed message
/// </summary>
/// <remarks>
/// This is never returned by the message processor stream loop
/// which uses the <see cref="MessageProcessor.RawLine"/> event instead,
/// it is only used in <see cref="MessageProcessor.ParseMessage(string)"/>
/// when an unknown (but valid) message is given
/// </remarks>
public class RawMessage : BaseMessage
{
    /// <summary>
    /// The raw message line, minus the leading '$' and checksum
    /// </summary>
    /// <remarks>
    /// Use <see cref="ToString"/> to convert this instance into a fully formed line with checksum
    /// </remarks>
    public string RawLine { get; }

    internal RawMessage(string line) : base(line)
    {
        RawLine = line;
    }

    /// <summary>
    /// Returns the raw NMEA line for this message including a valid checksum,
    /// if the source checksum was valid
    /// </summary>
    /// <returns>NMEA Message with checksum</returns>
    public override string ToString()
    {
        //Append calculated checksum if the original message had none,
        //or if it had a valid checksum
        if (!ChecksumPresent || ChecksumValid)
        {
            var checksum = 0;
            foreach (var c in RawLine)
            {
                checksum ^= c;
            }
            return $"${RawLine}*{checksum:X2}";
        }
        //If the source had an invalid checksum, forward the invalid checksum,
        //otherwise we could turn a faulty message into a valid one
        return $"${RawLine}*{Checksum:X2}";
    }
}
