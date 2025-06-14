using MessageDecoder.Messages;

namespace MessageDecoder;

/// <summary>
/// Delegate for an error event
/// </summary>
/// <param name="sender">
/// Component that generated the event.
/// Likely a <see cref="MessageProcessor"/> instance.</param>
/// <param name="errorType">The type of error reported</param>
/// <param name="exception">If available, the underlying exception that was generated</param>
public delegate void ReceiverErrorDelegate(object sender, ReceiveErrorType errorType, Exception? exception);

/// <summary>
/// Delegate for a raw line event
/// </summary>
/// <param name="sender">
/// Component that generated the event.
/// Likely a <see cref="MessageProcessor"/> instance.
/// </param>
/// <param name="line">Raw line</param>
/// <param name="lineValid">
/// Whether the system considers the line to be valid
/// </param>
/// <param name="checksumValid">
/// Whether the system considers the checksum to be valid.
/// This will always be "false" if the line is not valid
/// </param>
public delegate void RawLineDelegate(object sender, string line, bool lineValid, bool checksumValid);

/// <summary>
/// Delegate for when a raw message was known and successfully parsed into a message object
/// </summary>
/// <param name="sender">
/// Component that generated the event.
/// Likely a <see cref="MessageProcessor"/> instance.
/// </param>
/// <param name="message">Processed message</param>
public delegate void MessageDelegate(object sender, BaseMessage message);