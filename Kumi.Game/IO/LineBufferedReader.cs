using System.Text;

namespace Kumi.Game.IO;

public class LineBufferedReader : IDisposable
{
    private readonly StreamReader reader;
    
    private string? peekedLine;

    public LineBufferedReader(Stream stream, bool leaveOpen = false)
    {
        reader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen);
    }
    
    public void Seek(long offset, SeekOrigin origin)
    {
        reader.DiscardBufferedData();
        reader.BaseStream.Seek(offset, origin);
        peekedLine = null;
    }
    
    public string? PeekLine()
        => peekedLine ??= reader.ReadLine();
    
    public string? ReadLine()
    {
        var line = peekedLine ?? reader.ReadLine();
        
        peekedLine = null;
        return line;
    }

    public string ReadToEnd()
    {
        if (peekedLine == null)
            return reader.ReadToEnd();

        var builder = new StringBuilder();
        builder.AppendLine(peekedLine);
        builder.AppendLine(reader.ReadToEnd());

        return builder.ToString();
    }

    public void Dispose()
    {
        reader.Dispose();
    }
}
