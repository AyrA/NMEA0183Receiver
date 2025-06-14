# NMEA0183 Receiver

A 100% dependency free library that processes NMEA 0183 messages,
these are commonly seen in GPS devices.

Note: This document as well as most code comments use the terms GPS or satnav.
In general this implementation is not limited to GPS specifically,
and will correctly deal with receivers that generate messages for other navigation systems.

## Installation

1. Clone the repository
2. Build MessageDecoder in release mode

## Usage

1. Add the built dll from the previous chapter as reference to your project
2. Create an instance of `MessageProcessor` or use the static `ParseMessage` method
3. If using an instance, hook up the events you need.

The most common usage is likely to process messages from a GPS receiver
which is exposed via virtual serial port.

The message processor expects a TextReader as argument.
You can create a minimal wrapper around a `SerialPort` instance
that forwards `ReadLine()`. Other methods are not needed by the message processor.

The `SerialReader` in the main project is a mostly complete implementation
you can use as-is.

As an alternative, just read the lines yourself
and use the static `ParseMessage` method.

Example:

```csharp
void ReadLoop(SerialPort sp, CancellationToken ct)
{
	while(!ct.IsCancellationRequested)
	{
		var msg = MessageProcessor.ParseMessage(sp.ReadLine());
		if(msg is RecommendedMinumumMessage rmc)
		{
			//Use properties of "rmc" here
		}
	}
}
```

### Potential problems

You may encounter the following problems:

#### No Messages

If you receive no messages, check if the serial parameters are correct
(see next chapter for details).
Some receivers may need the almanac data before they start sending messages.
It can take up to 15 minutes for this data to be received.

#### Port Configuration

Do not forget to call `.Open()` on the serial port before trying to read from it.
Be aware that the `MessageProcessor` instance immediately starts to read
when you use the constructor,
so make sure whatever text reader implementation you use
is ready to read from the underlying stream.
If a read fails the processor will raise an error event and stop reading.

#### Garbled or Partial Messages

It is normal for the first message to be garbled or only partially available,
because most receivers generate serial messages even if no reader is connected.
When you open the serial port you sometimes get whatever is currently in the buffer,
or the device may just cut you into the stream halfway through a line.
In other words, it's not uncommon for the message processor to raise a few error
events before it raises the first message event.

If your messages are faulty for an extended period it may indicate a bad connection, or wrong serial port parameters.

#### Messages Stop

This usually happens when you don't process them fast enough
and the internal serial buffer of the device overflows.
Toggling the RTS and DTR pins off and on a few times can sometimes fix this.
If not, the serial connection likely has to be closed and reestablished.

In general, you should never stop processing messages.
The message processor has an `EnableRaisingEvents` property.
You can set it to false to stop all events.
The processor still reads messages but will silently discard them
to keep buffers empty.

#### Invalid Messages

If a large chunk of the messages are invalid
your device may not be able to see enough of the sky.
It may fail to get its position, and as a result,
generate mostly empty messages. Many messages have a time component.
If the time approximately matches the current UTC time
even though the message is considered invalid,
it can be a sign that the device gets at least some signal reception.
Sometimes the device needs a new almanac.
This usually takes less than 15 minutes to correct itself.

#### Invalid Date

If the date in the `RecommendedMinumumMessage` is approximately 20 years in the past your device may have missed the last week number rollover.
Wait for 20 minutes and see if the date corrects itself.
If it doesn't, your device doesn't understands the updated week number system.
In those cases, you can manually add 1024 weeks to the date value.
If your device is very old you may need to do this twice.

## NMEA Serial Defaults

If your device is NMEA 0183 compatible it likely works with these parameters:

|Item       | Value |
|-----------|-------|
| Baud rate | 4800  |
| Parity    | None  |
| Data bits | 8     |
| Stop bits | 1     |
| Handshake | RTS   |

Note: Some receivers allow higher baud rates.
This is especially useful for devices that generate too many messages.
By default, most devices generate each message type once per second.
If too many message types are sent, the baud rate may not be large enough
to fit them all within the time window.
In those cases, increasing the baud rate may be useful,
but this is not supported by all receivers.

### Relevant Messages

Most receivers will send `RecommendedMinumumMessage` messages.
This message contains everything needed for most usage scenarios.

Always check the `IsValid` property.
It is present in all messages that implement `BaseSatnavMessage`

## Extending

The library already contains implementations of common message types.

It's fairly trivial to create your own message.
You can either implement `BaseMessage` or `BaseSatnavMessage`

Both of these types only require you
to pass the line string in their constructor.
`BaseMessage` is the base of all messages.
`BaseSatnavMessage` is the base of all messages generated by a GPS device.

If the message type (the first 2 characters directly after '$')
are one of the following: "BD,GB,GA,GP,GL,LC",
then you likely want `BaseSatnavMessage`

Finally, you simply need to add your type
into the "types" list of the `LineUtils` class.
