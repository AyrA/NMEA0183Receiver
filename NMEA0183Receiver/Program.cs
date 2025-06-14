using MessageDecoder;
using MessageDecoder.Messages;
using NMEA0183Receiver;
using System.IO.Ports;

#if DEBUG
//In general, lower ports are real serial ports while higher numbers are virtual ports
args = [SerialPort.GetPortNames().Order().Last()];
#endif

if (args.Length == 0 || args.Contains("/?"))
{
    Console.WriteLine("{0} <SerialPort>", Path.GetFileName(Environment.ProcessPath ?? "NMEA0183Receiver"));
    return 0;
}

using var sp = new SerialPort("COM7", 4800, Parity.None, 8, StopBits.One);
sp.Handshake = Handshake.RequestToSend;
sp.Open();
using var reader = new SerialReader(sp);
using var decoder = new MessageProcessor(reader, false);
decoder.RawLine += Decoder_RawLine;
decoder.ReceiverError += Decoder_ReceiverError;
decoder.Message += Decoder_Message;

do
{
    Console.WriteLine("Press [ESC] to exit");
} while (Console.ReadKey(true).Key != ConsoleKey.Escape);

return 0;

static bool DumpMessage(BaseMessage message)
{
    if (message is PositionMessage pos)
    {
        Console.WriteLine("Lat: {0}; Long: {1}; Alt: {2}", pos.Latitude, pos.Longitude, pos.Altitude);
    }
    else if (message is DillutionOfPrecisionMessage dop)
    {
        Console.WriteLine("DOP: {0}; H: {1}; V: {2}; Sources: {3}",
            dop.DillutionOfPrecision,
            dop.HorizontalDillution,
            dop.VerticalDillution,
            string.Join(',', dop.Satellites));
    }
    else if (message is SatelliteViewMessage svm)
    {
        Console.WriteLine("View {0}/{1}; Count: {2}; Data: {3}",
            svm.Sentence, svm.NumberOfSentences, svm.SatellitesInView,
            string.Join(',', svm.Satellites.AsEnumerable()));
    }
    else if (message is RecommendedMinumumMessage rmc)
    {
        Console.WriteLine("Date: {0}, Lat: {1}; Long: {2}; Speed: {3} m/s; Heading: {4}",
            rmc.FixTaken.ToLocalTime(),
            rmc.Latitude, rmc.Longitude,
            rmc.SpeedMeters, rmc.TrackAngle);
    }
    else if (message is GeographicLatitudeLongitude gll)
    {
        Console.WriteLine("Time: {0}, Lat: {1}; Long: {2}",
            gll.FixTaken,
            gll.Latitude, gll.Longitude);
    }
    else
    {
        return false;
    }
    return true;
}

static void Decoder_Message(object sender, BaseMessage message)
{
    if (!DumpMessage(message))
    {
        Console.WriteLine("Parsed message but did not dump: {0}", message.MessageType);
    }
}

static void Decoder_ReceiverError(object sender, ReceiveErrorType errorType, Exception? exception)
{
    Console.WriteLine("ERROR {0}: {1}", errorType, exception?.Message ?? "<no exception data>");
}

static void Decoder_RawLine(object sender, string line, bool lineValid, bool checksumValid)
{
    if (lineValid)
    {
        Console.ForegroundColor = checksumValid ? ConsoleColor.Green : ConsoleColor.Yellow;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }
    Console.WriteLine(line);
    Console.ResetColor();
}
