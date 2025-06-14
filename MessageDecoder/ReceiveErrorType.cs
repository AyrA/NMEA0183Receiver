namespace MessageDecoder;

/// <summary>
/// Error types used in <see cref="MessageProcessor.ReceiverError"/> event
/// </summary>
public enum ReceiveErrorType
{
    /// <summary>
    /// Task is null
    /// </summary>
    /// <remarks>
    /// This achievement is only obtainable if you mess around with reflection.
    /// No further lines will be read
    /// </remarks>
    TaskNull,
    /// <summary>
    /// The read line task faulted
    /// </summary>
    /// <remarks>No further lines will be read</remarks>
    TaskFail,
    /// <summary>
    /// Source stream returned a null string
    /// </summary>
    /// <remarks>
    /// This indicates EOF, an internal abort, or error in the stream.
    /// No further lines will be read
    /// </remarks>
    LineNull,
    /// <summary>
    /// The line initially matched a known message type but failed to parse.
    /// </summary>
    LineInvalid
}
