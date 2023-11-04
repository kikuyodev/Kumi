using System.Globalization;
using Kumi.Game.Charts.Events;
using Kumi.Game.Charts.Timings;
using Kumi.Game.IO.Formats;

namespace Kumi.Game.Charts.Formats;

public class ChartEncoder : FileEncoder<Chart, ChartSections>
{
    public ChartEncoder()
        : base(Chart.CURRENT_VERSION)
    {
    }
    
    protected override void HandleSection(ChartSections section)
    {
        switch (section)
        {
            case ChartSections.Metadata:
                handleMetadata();
                break;
            case ChartSections.Chart:
                handleChart();
                break;
            case ChartSections.Events:
                handleEvents();
                break;
            case ChartSections.Timings:
                handleTimings();
                break;
            case ChartSections.Notes:
                handleNotes();
                break;
        }
    }

    private void handleMetadata()
    {
        WriteLine($"ARTIST = {Current.Metadata.Artist}");
        WriteLine($"ARTIST_ROMANISED = {Current.Metadata.ArtistRomanised}");
        WriteLine($"TITLE = {Current.Metadata.Title}");
        WriteLine($"TITLE_ROMANISED = {Current.Metadata.TitleRomanised}");
        WriteLine($"SOURCE = {Current.Metadata.Source}");
        WriteLine($"GENRE = {Current.Metadata.Genre}");
        WriteLine($"TAGS = {Current.Metadata.Tags}");
        WriteLine($"CREATOR = {Current.Metadata.Creator.Username}");
        WriteLine($"DIFFICULTY_NAME = {Current.ChartInfo.DifficultyName}");
        WriteLine($"PREVIEW_POINT = {Current.Metadata.PreviewTime}");
    }

    private void handleChart()
    {
        WriteLine($"INITIAL_SCROLL_SPEED = {Current.ChartInfo.InitialScrollSpeed}");
        WriteLine($"MUSIC_FILE = {Current.Metadata.AudioFile}");
    }

    private void handleEvents()
    {
        foreach (var e in Current.Events)
        {
            if (e is SetMediaEvent setMedia)
            {
                WriteLine($"{((int)e.Type).ToString()},{encodeString(setMedia.FileName)}");
                continue;
            }
            
            var values = new List<string>
            {
                ((int)e.Type).ToString(),
                e.StartTime.ToString(CultureInfo.InvariantCulture),
            };

            if (e is IHasEndTime endTime)
                values.Add(endTime.EndTime.ToString(CultureInfo.InvariantCulture));
            
            if (e is IHasMedia media)
                values.Add(encodeString(media.FileName));

            // special cases
            // TODO: probably generalize these to interfaces?
            if (e is SwitchMediaEvent switchMediaEvent)
                values.Add(switchMediaEvent.Easing.ToString());

            if (e is DisplayLyricEvent lyricEvent)
            {
                values.Add(lyricEvent.Easing.ToString());
                values.Add(lyricEvent.CrossfadeTime.ToString(CultureInfo.InvariantCulture));
                values.Add(encodeString(lyricEvent.Lyric));
            }
            
            WriteLine(string.Join(",", values));
        }
    }

    private void handleTimings()
    {
        foreach (var point in Current.TimingPoints)
        {
            var values = new List<string>
            {
                ((int)point.PointType).ToString(),
                point.StartTime.ToString(CultureInfo.InvariantCulture),
            };

            if (point is UninheritedTimingPoint uninherited)
            {
                values.Add(uninherited.BPM.ToString(CultureInfo.InvariantCulture));
                values.Add(uninherited.TimeSignature.ToString());
            }
            
            values.Add(point.RelativeScrollSpeed.ToString(CultureInfo.InvariantCulture));
            values.Add(point.Volume.ToString(CultureInfo.InvariantCulture));
            values.Add(((int)point.Flags).ToString());
            
            WriteLine(string.Join(",", values));
        }
    }

    private void handleNotes()
    {
        foreach (var note in Current.Notes)
        {
            var values = new List<string>
            {
                ((int)note.Type).ToString(),
                note.StartTime.ToString(CultureInfo.InvariantCulture),
            };
            
            if (note is IHasEndTime endTime)
                values.Add(endTime.EndTime.ToString(CultureInfo.InvariantCulture));
            
            values.Add(((int)note.Flags).ToString());
            
            WriteLine(string.Join(",", values));
        }
    }
    
    private string encodeString(string input)
        => $"\"{input}\"";

    protected override void HandleHeader()
        => WriteLine($"#KUMI CHART FORMAT v{Version}");
}
