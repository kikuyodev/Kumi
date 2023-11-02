using System.Text;
using System.Text.RegularExpressions;
using Kumi.Game.Extensions;
using osu.Framework.Logging;

namespace Kumi.Game.IO.Formats;

/// <summary>
/// A parser for a file format, which can be used to parse a file into a <see cref="T"/>.
/// </summary>
/// <typeparam name="T">The class to parse into.</typeparam>
/// <typeparam name="TSection">An enum of sections that are parseable</typeparam>
public abstract class FileDecoder<T, TSection> : FileHandler<T, TSection>
    where T : new()
    where TSection : struct
{
    protected FileDecoder(int version)
        : base(version)
    {
        
    }
    
    public new T Decode(Stream stream) => Decode(stream, new T());

    /// <summary>
    /// Parses a readable stream into a designated output of type <see cref="T"/>.
    /// </summary>
    /// <param name="stream">The stream to parse.</param>
    public new T Decode(Stream stream, T input)
    {
        // TODO: Split this more to make it less boilerplate-y.
        using var reader = new LineBufferedReader(stream);
        
        string header = reader.ReadLine();
        if (!ValidateHeader(header))
            throw new InvalidDataException($"The header of the file being parsed by {nameof(FileDecoder<T, TSection>)} is invalid: {header}");
        
        CurrentSection = default;
        Current = input;
        string line;
        
        Process(input);
        
        while ((line = reader.ReadLine()) != null)
        {
            if (ShouldIgnoreLine(line))
                continue;

            if (ShouldStripComments(CurrentSection))
                stripComments(line);

            line = line.TrimEnd();

            // Sections start with a '[#' and end with a ']'.
            // TODO: Potentially make this more dynamic if we make formats with more than one section header.
            if (isSectionHeader(line))
            {
                TSection section = default;
                
                if (!tryGetSection(line, out section))
                    Logger.Log($"An unknown section was found: {line}", LoggingTarget.Runtime, LogLevel.Important);
                
                if (CurrentSection.Equals(section))
                    continue;
                
                CurrentSection = section;
                continue;
            }

            try
            {
                ProcessLine(line);
            } catch (Exception e)
            {
                Logger.Log($"An error occurred while parsing a line: {line}", LoggingTarget.Runtime, LogLevel.Error);
                Logger.Log(e.Message, LoggingTarget.Runtime, LogLevel.Error);
            }
        }
        
        PostProcess();

        return Current;
    }

    protected override void Process(T input)
    {
        return;
    }

    /// <summary>
    /// Processes a line of the file.
    /// </summary>
    /// <param name="line">The line being processed.</param>
    protected virtual void ProcessLine(string line)
    {
        return;
    }
    
    /// <summary>
    /// Whether or not to strip comments from lines in this particular section.
    /// </summary>
    /// <param name="section">The section being checked.</param>
    protected virtual bool ShouldStripComments(TSection section) => true;
    
    /// <summary>
    /// Whether or not to ignore this line.
    /// </summary>
    /// <param name="line">The line being checked.</param>
    protected virtual bool ShouldIgnoreLine(string line) => string.IsNullOrWhiteSpace(line) || line.StartsWith("#");
    
    /// <summary>
    /// Safely validates the header of a file if it is the expected format.
    /// </summary>
    /// <param name="header">The header to expect.</param>
    protected virtual bool ValidateHeader(string header) => !string.IsNullOrWhiteSpace(header) && new Regex(@"^#KUMI\sv[0-9]+$").IsMatch(header);
    
    private bool isSectionHeader(string line) => line.StartsWith(SectionHeader.Start) && line.EndsWith(SectionHeader.End);

    private bool tryGetSection(string line, out TSection section)
    {
        section = default(TSection);
        if (!isSectionHeader(line))
            return false;

        return Enum.TryParse<TSection>(sanitizeHeader(line), out section);
    }

    private string stripComments(string line)
    {
        var commentIndex = line.IndexOf(CommentCharacter, StringComparison.Ordinal);
        if (commentIndex == -1)
            return line;

        return line.Substring(0, commentIndex);
    }
    
    private string sanitizeHeader(string line)
    {
        if (!isSectionHeader(line))
            return line;

        var sectionName = stripHeader(line);
        
        if (sectionName.IsPascalCase())
            return line;

        if (sectionName.ToLower().IsSnakeCase())
        {
            var sb = new StringBuilder();
            var words = sectionName.ToLower().Split('_');
            foreach (var word in words)
            {
                sb.Append(char.ToUpper(word[0]));
                sb.Append(word.Substring(1));
            }

            return $"{sb}";
        }

        if (sectionName.IsCamelCase())
        {
            // Should just be able to capitalize the first letter.
            return $"{char.ToUpper(sectionName[0])}{sectionName.Substring(1)}";
        }
        
        // If it's not camel or snake case, just return the line as-is.
        // I'm sure we're past the point of caring anymore.
        return sectionName;
    }
    
    private string stripHeader(string line)
    {
        if (!isSectionHeader(line))
            return line;

        return line.Substring(SectionHeader.Start.Length, line.Length - SectionHeader.End.Length - SectionHeader.Start.Length);
    }
}

/// <summary>
/// A line-based file decoder for a specific file format.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class FileDecoder<T> : FileHandler<T>
    where T : new()
{
    public FileDecoder(int version)
        : base(version)
    {
    }
    
    public T Decode(Stream stream) => Decode(stream, new T());

    /// <summary>
    /// Parses a readable stream into a designated output of type <see cref="T"/>.
    /// </summary>
    /// <param name="stream">The stream to parse.</param>
    public virtual T Decode(Stream stream, T input)
    {
        using var reader = new LineBufferedReader(stream);
        
        string header = reader.ReadLine();
        if (!ValidateHeader(header))
            throw new InvalidDataException($"The header of the file being parsed by {nameof(FileDecoder<T>)} is invalid: {header}");
        
        Current = input;
        string line;
        
        while ((line = reader.ReadLine()) != null)
        {
            if (ShouldIgnoreLine(line))
                continue;

            line = line.Trim();

            try
            {
                ProcessLine(line);
            } catch (Exception e)
            {
                Logger.Log($"An error occurred while parsing a line: {line}", LoggingTarget.Runtime, LogLevel.Error);
                Logger.Log(e.Message, LoggingTarget.Runtime, LogLevel.Error);
            }
        }
        
        PostProcess();

        return Current;
    }

    /// <summary>
    /// Safely validates the header of a file if it is the expected format.
    /// </summary>
    /// <param name="header">The header to expect.</param>
    protected virtual bool ValidateHeader(string header) => !string.IsNullOrWhiteSpace(header) && new Regex(@"^#KUMI\sv[0-9]+$").IsMatch(header);
    
    /// <summary>
    /// Processes a line of the file.
    /// </summary>
    /// <param name="line">The line being processed.</param>
    protected virtual void ProcessLine(string line)
    {
        return;
    }
    
    /// <summary>
    /// Whether or not to ignore this line.
    /// </summary>
    /// <param name="line">The line being checked.</param>
    protected virtual bool ShouldIgnoreLine(string line) => string.IsNullOrWhiteSpace(line) || line.StartsWith("#");
    
    private string stripComments(string line)
    {
        var commentIndex = line.IndexOf(CommentCharacter, StringComparison.Ordinal);
        if (commentIndex == -1)
            return line;

        return line.Substring(0, commentIndex);
    }
   
}