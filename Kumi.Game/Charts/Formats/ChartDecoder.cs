using System.Text.RegularExpressions;
using Kumi.Game.Charts.Events;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Charts.Timings;
using Kumi.Game.IO.Formats;
using Kumi.Game.Models;
using Kumi.Game.Utils;

namespace Kumi.Game.Charts.Formats;

public class ChartDecoder : FileDecoder<Chart, ChartSections>
{
    public ChartDecoder()
        : base(Chart.CURRENT_VERSION)
    {
    }

    protected override void ProcessLine(string line)
    {
        switch (CurrentSection)
        {
            case ChartSections.Metadata:
                processMetadata(line);
                break;

            case ChartSections.Chart:
                processChart(line);
                break;

            case ChartSections.Events:
                processEvents(line);
                break;

            case ChartSections.Timings:
                processTimings(line);
                break;

            case ChartSections.Notes:
                processNotes(line);
                break;
        }
    }

    protected override void PostProcess()
    {
        if (Current.Events.Any())
        {
            var bgEvent = Current.Events.FirstOrDefault(e => e.Type == EventType.SetMedia);
            if (bgEvent == null)
                return;

            Current.Metadata.BackgroundFile = ((SetMediaEvent) bgEvent).FileName;
        }

        if (string.IsNullOrEmpty(Current.Metadata.ArtistRomanised))
            Current.Metadata.ArtistRomanised = Current.Metadata.Artist;

        if (string.IsNullOrEmpty(Current.Metadata.TitleRomanised))
            Current.Metadata.TitleRomanised = Current.Metadata.Title;
    }

    private void processMetadata(string line)
    {
        var pair = parseKeyValuePair(line);

        switch (pair.Key)
        {
            case "artist":
                Current.Metadata.Artist = pair.Value;
                break;

            case "artist_romanised":
                Current.Metadata.ArtistRomanised = pair.Value;
                break;

            case "title":
                Current.Metadata.Title = pair.Value;
                break;

            case "title_romanised":
                Current.Metadata.TitleRomanised = pair.Value;
                break;

            case "source":
                Current.Metadata.Source = pair.Value;
                break;

            case "genre":
                Current.Metadata.Genre = pair.Value;
                break;

            case "tags":
                Current.Metadata.Tags = pair.Value;
                break;

            case "creator":
                Current.Metadata.Creator = new RealmUser { Username = pair.Value };
                break;

            case "difficulty_name":
                Current.ChartInfo.DifficultyName = pair.Value;
                break;

            case "preview_point":
                Current.Metadata.PreviewTime = StringUtils.AssertAndFetch<int>(pair.Value);
                break;
        }
    }

    private void processChart(string line)
    {
        var pair = parseKeyValuePair(line);

        switch (pair.Key)
        {
            case "initial_scroll_speed":
                Current.ChartInfo.InitialScrollSpeed = StringUtils.AssertAndFetch<float>(pair.Value);
                break;

            case "music_file":
                Current.Metadata.AudioFile = pair.Value;
                break;
        }
    }

    private void processEvents(string line)
    {
        var args = line.SplitComplex(Event.DELIMITER).ToArray();
        var typeValue = (EventType) StringUtils.AssertAndFetch<int>(args[0]);
        Event ev = typeValue switch
        {
            EventType.SetMedia => new SetMediaEvent(),
            EventType.SwitchMedia => new SwitchMediaEvent(),
            EventType.Break => new BreakTimeEvent(),
            EventType.KiaiTime => new KiaiTimeEvent(),
            EventType.DisplayLyric => new DisplayLyricEvent(),
            _ => throw new InvalidDataException($"Invalid event type: {typeValue}")
        };

        ev.ParseFrom(args);
        Current.Events.Add(ev);
    }

    private void processTimings(string line)
    {
        var args = line.SplitComplex(TimingPoint.DELIMITER).ToArray();
        var typeValue = (TimingPointType) StringUtils.AssertAndFetch<int>(args[0]);
        TimingPoint? tp;

        switch (typeValue)
        {
            case TimingPointType.Inherited:
                tp = new InheritedTimingPoint(StringUtils.AssertAndFetch<float>(args[1]));
                tp.RelativeScrollSpeed = StringUtils.AssertAndFetch<float>(args[2]);
                tp.Volume = StringUtils.AssertAndFetch<int>(args[3]);
                tp.Flags = (TimingFlags) StringUtils.AssertAndFetch<int>(args[4]);
                break;

            case TimingPointType.Uninherited:
                var timingPoint = new UninheritedTimingPoint(StringUtils.AssertAndFetch<int>(args[1]));
                timingPoint.BPM = StringUtils.AssertAndFetch<float>(args[2]);

            {
                var measure = args[3];
                var split = measure.Split('/');

                if (split.Length != 2)
                    throw new InvalidDataException($"Invalid measure: {measure}");

                var nominator = StringUtils.AssertAndFetch<int>(split[0]);
                var denominator = StringUtils.AssertAndFetch<int>(split[1]);

                timingPoint.TimeSignature = new TimeSignature(nominator, denominator);
            }

                timingPoint.RelativeScrollSpeed = StringUtils.AssertAndFetch<float>(args[4]);
                timingPoint.Volume = StringUtils.AssertAndFetch<int>(args[5]);
                timingPoint.Flags = (TimingFlags) StringUtils.AssertAndFetch<int>(args[6]);

                tp = timingPoint;
                break;

            default:
                throw new InvalidDataException($"Invalid timing point type: {typeValue}");
        }

        Current.TimingPoints.Add(tp);
    }

    private void processNotes(string line)
    {
        var args = line.SplitComplex(Note.DELIMITER).ToArray();
        var typeValue = (NoteType) StringUtils.AssertAndFetch<int>(args[0]);
        Note? note;

        switch (typeValue)
        {
            case NoteType.Don:
            case NoteType.Kat:
                note = new DrumHit(StringUtils.AssertAndFetch<float>(args[1]));
                note.Flags = (NoteFlags) StringUtils.AssertAndFetch<int>(args[2]);
                break;

            case NoteType.Drumroll:
                var drumroll = new DrumRoll(StringUtils.AssertAndFetch<float>(args[1]));
                drumroll.EndTime = StringUtils.AssertAndFetch<float>(args[2]);
                drumroll.Flags = (NoteFlags) StringUtils.AssertAndFetch<int>(args[3]);

                note = drumroll;
                break;

            case NoteType.Balloon:
                var balloon = new Balloon(StringUtils.AssertAndFetch<float>(args[1]));
                balloon.EndTime = StringUtils.AssertAndFetch<float>(args[2]);
                balloon.Flags = (NoteFlags) StringUtils.AssertAndFetch<int>(args[3]);

                note = balloon;
                break;

            default:
                throw new InvalidDataException($"Invalid note type: {typeValue}");
        }

        // TODO: Temporary
        note.Windows = new NoteWindows();
        note.Type = typeValue;
        Current.Notes.Add(note);
    }

    protected override bool ValidateHeader(string header) => !string.IsNullOrWhiteSpace(header) && new Regex(@"^#KUMI CHART FORMAT\sv[0-9]+$").IsMatch(header);

    private KeyValuePair<string, string> parseKeyValuePair(string line)
    {
        var split = line.Split('=');
        if (split.Length != 2)
            throw new InvalidDataException($"Invalid line: {line}");

        var key = split[0].ToLower().Trim();
        var value = split[1].Trim();

        return new KeyValuePair<string, string>(key, value);
    }
}

public enum ChartSections
{
    Metadata,
    Chart,
    Events,
    Timings,
    Notes
}
