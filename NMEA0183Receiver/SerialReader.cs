using System.IO.Ports;

namespace NMEA0183Receiver;

internal class SerialReader(SerialPort sp) : TextReader
{
    public override int Peek()
        => throw new NotSupportedException("Cannot peek on serial stream");

    public override int Read()
        => sp.ReadChar();

    public override int Read(char[] buffer, int index, int count)
        => sp.Read(buffer, index, count);

    public override int ReadBlock(char[] buffer, int index, int count)
        => Read(buffer, index, count);

    public override string? ReadLine()
        => sp.ReadLine();

    public override string ReadToEnd()
        => throw new NotSupportedException("Serial streams have no end");

    public override string ToString()
        => $"{nameof(SerialReader)} on port {sp.PortName}";

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            sp.Dispose();
        }
        base.Dispose(disposing);
    }
}
