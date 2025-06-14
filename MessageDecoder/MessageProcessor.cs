using MessageDecoder.Messages;
using System.Diagnostics;

namespace MessageDecoder;

/// <summary>
/// Processes NMEA 0183 messages
/// </summary>
public class MessageProcessor : IDisposable
{
    /// <summary>
    /// Triggered whenever something with the receiver is not right
    /// </summary>
    public event ReceiverErrorDelegate ReceiverError = delegate { };

    /// <summary>
    /// Triggered for every line received regardless of whether it's valid or not
    /// </summary>
    public event RawLineDelegate RawLine = delegate { };

    /// <summary>
    /// Triggered when a line resembles a known message
    /// and was successfully parsed into a message object
    /// </summary>
    public event MessageDelegate Message = delegate { };

    private bool disposed = false;
    private readonly TextReader dataSource;
    private readonly bool ownsStream;

    /// <summary>
    /// If enabled, will not trigger <see cref="RawLine"/> event if a
    /// <see cref="Message"/> event was raised,
    /// effectively turing <see cref="RawLine"/> into an "unknown line" event.
    /// </summary>
    /// <remarks>Default: true</remarks>
    public bool SuppressRawLineEventsForProcessedMessages { get; set; } = true;

    /// <summary>
    /// Enables events. Disabling this will prevent any and all events from being raised.
    /// Received lines are simply discarded unprocessed.
    /// </summary>
    /// <remarks>Default: true</remarks>
    public bool EnableRaisingEvents { get; set; } = true;

    public MessageProcessor(TextReader dataSource, bool ownsStream)
    {
        this.dataSource = dataSource;
        this.ownsStream = ownsStream;
        dataSource.ReadLineAsync().ContinueWith(ProcessLine);
    }

    private void ProcessLine(Task<string?> result)
    {
        bool processed = false;
        if (disposed)
        {
            return;
        }

        if (result == null)
        {
            if (EnableRaisingEvents)
            {
                ReceiverError(this, ReceiveErrorType.TaskNull, null);
            }

            return;
        }
        if (result.IsFaulted)
        {
            if (EnableRaisingEvents)
            {
                ReceiverError(this, ReceiveErrorType.TaskFail, result.Exception);
            }

            return;
        }
        var line = result.Result?.Trim(); //Line may contain stray whitespace, especially at the end.
        if (line == null)
        {
            if (EnableRaisingEvents)
            {
                ReceiverError(this, ReceiveErrorType.LineNull, null);
            }

            return;
        }
        if (EnableRaisingEvents)
        {
            //Process line
            if (line.StartsWith('$'))
            {
                bool checksumValid = false;
                //Line is valid
                if (LineUtils.ChecksumValidator().IsMatch(line))
                {
                    //Line has checksum
                    var checksum = int.Parse(line[^2..], System.Globalization.NumberStyles.HexNumber);
                    foreach (var c in line[1..^3])
                    {
                        checksum ^= c;
                    }
                    checksumValid = checksum == 0;
                }
                try
                {
                    var m = LineUtils.Parse(line);
                    if (m != null)
                    {
                        processed = true;
                        Message(this, m);
                    }
                }
                catch (Exception ex)
                {
                    ReceiverError(this, ReceiveErrorType.LineInvalid, ex);
                }
                if (!(SuppressRawLineEventsForProcessedMessages && processed))
                {
                    RawLine(this, line, true, checksumValid);
                }
            }
            else
            {
                RawLine(this, line, false, false);
            }
        }
        else
        {
            //Send lines to the debug listeners. Note: This is a no-op in release mode
            Debug.Print(line);
        }
        dataSource.ReadLineAsync().ContinueWith(ProcessLine);
    }

    public void Dispose()
    {
        disposed = true;
        GC.SuppressFinalize(this);
        if (ownsStream)
        {
            dataSource.Dispose();
        }
    }

    /// <inheritdoc cref="LineUtils.Parse(string)" />
    public static BaseMessage? ParseMessage(string line)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(line);
        return LineUtils.Parse(line.Trim());
    }
}
