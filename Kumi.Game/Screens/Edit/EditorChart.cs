using System.Collections;
using Kumi.Game.Charts;
using Kumi.Game.Charts.Events;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Timings;
using osu.Framework.Bindables;

namespace Kumi.Game.Screens.Edit;

public partial class EditorChart : TransactionalCommitComponent, IChart
{
    public event Action<Note>? NoteAdded;
    public event Action<Note>? NoteRemoved;
    public event Action<Note>? NoteUpdated;

    public readonly BindableList<Note> SelectedNotes = new BindableList<Note>();
    
    public readonly Bindable<Note?> PlacementNote = new Bindable<Note?>();

    private readonly ChartInfo chartInfo;
    private readonly IChart playableChart;
    
    public BindableInt PreviewTime { get; }

    private readonly Dictionary<Note, Bindable<double>> startTimeBindable = new Dictionary<Note, Bindable<double>>();

    public EditorChart(IChart playableChart, ChartInfo? chartInfo = null)
    {
        this.chartInfo = chartInfo ?? playableChart.ChartInfo;
        this.playableChart = playableChart;

        foreach (var note in Notes)
            trackStartTime((Note) note);

        PreviewTime = new BindableInt(this.chartInfo.Metadata.PreviewTime);
        PreviewTime.BindValueChanged(s =>
        {
            BeginChange();
            this.chartInfo.Metadata.PreviewTime = s.NewValue;
            EndChange();
        });
    }

    public ChartInfo ChartInfo
    {
        get => chartInfo;
        set => throw new InvalidOperationException("Cannot set ChartInfo of EditorChart.");
    }

    public ChartMetadata Metadata => chartInfo.Metadata;
    public IReadOnlyList<IEvent> Events => playableChart.Events;
    public IReadOnlyList<ITimingPoint> TimingPoints => playableChart.TimingPoints;
    public IReadOnlyList<INote> Notes => playableChart.Notes;
    
    public TimingPointHandler TimingPointHandler => ((Chart) playableChart).TimingHandler;

    private IList mutableNotes => (IList) playableChart.Notes;

    private readonly List<Note> batchPendingInsertions = new List<Note>();
    private readonly List<Note> batchPendingRemovals = new List<Note>();
    private readonly HashSet<INote> batchPendingUpdates = new HashSet<INote>();

    public void PerformOnSelection(Action<Note> action)
    {
        if (SelectedNotes.Count == 0)
            return;

        BeginChange();
        foreach (var n in SelectedNotes)
            action(n);
        EndChange();
    }

    public void Add(Note note)
    {
        var insertionIndex = findInsertionIndex(playableChart.Notes, note.StartTime);
        Insert(insertionIndex + 1, note);
    }
    
    public void AddRange(IEnumerable<Note> notes)
    {
        BeginChange();
        foreach (var n in notes)
            Add(n);
        EndChange();
    }

    public void Insert(int index, Note note)
    {
        trackStartTime(note);

        mutableNotes.Insert(index, note);

        BeginChange();
        batchPendingInsertions.Add(note);
        EndChange();
    }

    public void Update(INote note)
    {
        batchPendingUpdates.Add(note);
    }

    public void UpdateAllNotes()
    {
        foreach (var note in Notes)
            batchPendingUpdates.Add(note);
    }

    public bool Remove(Note note)
    {
        var index = FindIndex(note);

        if (index == -1)
            return false;
        
        RemoveAt(index);
        return true;
    }

    public void RemoveRange(IEnumerable<Note> notes)
    {
        BeginChange();
        foreach (var n in notes)
            Remove(n);
        EndChange();
    }

    public void RemoveAt(int index)
    {
        var note = (Note) mutableNotes[index]!;
        
        mutableNotes.RemoveAt(index);
        
        var bindable = startTimeBindable[note];
        bindable.UnbindAll();
        startTimeBindable.Remove(note);
        
        BeginChange();
        batchPendingRemovals.Add(note);
        EndChange();
    }

    public void Clear()
        => RemoveRange(Notes.ToArray().Cast<Note>());
    
    public int FindIndex(Note note)
        => mutableNotes.IndexOf(note);

    protected override void Update()
    {
        base.Update();

        if (batchPendingUpdates.Count > 0)
            UpdateState();
    }

    protected override void UpdateState()
    {
        if (batchPendingUpdates.Count == 0 && batchPendingRemovals.Count == 0 && batchPendingInsertions.Count == 0)
            return;

        foreach (var n in batchPendingRemovals) processNote(n);
        foreach (var n in batchPendingInsertions) processNote(n);
        foreach (var n in batchPendingUpdates) processNote((Note) n);

        var removals = batchPendingRemovals.ToArray();
        batchPendingRemovals.Clear();

        var inserts = batchPendingInsertions.ToArray();
        batchPendingInsertions.Clear();

        var updates = batchPendingUpdates.ToArray();
        batchPendingUpdates.Clear();

        foreach (var n in removals) SelectedNotes.Remove(n);

        foreach (var n in removals) NoteRemoved?.Invoke(n);
        foreach (var n in inserts) NoteAdded?.Invoke(n);
        foreach (var n in updates) NoteUpdated?.Invoke((Note) n);
    }

    private void processNote(Note note)
        => note.ApplyChartDefaults(playableChart);

    private void trackStartTime(Note note)
    {
        startTimeBindable[note] = note.StartTimeBindable.GetBoundCopy();
        startTimeBindable[note].ValueChanged += _ =>
        {
            mutableNotes.Remove(note);

            var insertionIndex = findInsertionIndex(playableChart.Notes, note.StartTime);
            mutableNotes.Insert(insertionIndex + 1, note);

            Update(note);
        };
    }

    private int findInsertionIndex(IReadOnlyList<INote> list, double startTime)
    {
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].StartTime > startTime)
                return i - 1;
        }

        return list.Count - 1;
    }
}
