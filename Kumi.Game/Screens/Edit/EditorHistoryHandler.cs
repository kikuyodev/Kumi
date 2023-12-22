using Kumi.Game.Charts;
using Kumi.Game.Charts.Formats;
using osu.Framework.Bindables;
using osu.Framework.Extensions;

namespace Kumi.Game.Screens.Edit;

/// <summary>
/// A class that handles copying and pasting of notes, timing points, and other objects;
/// as well as undoing and redoing actions.
/// </summary>
public partial class EditorHistoryHandler : TransactionalCommitComponent
{
    public const int MAX_SAVED_STATES = 50;
    
    public readonly Bindable<bool> CanUndo = new Bindable<bool>();
    public readonly Bindable<bool> CanRedo = new Bindable<bool>();

    public event Action? OnStateChange;
    
    private readonly List<byte[]> savedStates = new List<byte[]>();

    private int currentState = -1;
    private bool isRestoring;
    
    private readonly EditorChart editorChart;
    private readonly EditorChartPatcher patcher;

    public string CurrentStateHash
    {
        get
        {
            ensureStateSaved();

            using (var stream = new MemoryStream(savedStates[currentState]))
                return stream.ComputeSHA2Hash();
        }
    }
    
    /// <summary>
    /// A dictionary of the contents of the clipboard.
    /// </summary>
    public Dictionary<EditorClipboardType, BindableList<string>> Contents { get; } = new Dictionary<EditorClipboardType, BindableList<string>>();

    public EditorHistoryHandler(EditorChart editorChart)
    {
        this.editorChart = editorChart;
        patcher = new EditorChartPatcher(editorChart);
        
        Contents.Add(EditorClipboardType.Note, new BindableList<string>());

        editorChart.TransactionBegan += BeginChange;
        editorChart.TransactionEnded += EndChange;
        editorChart.SaveStateTriggered += SaveState;
    }

    #region Clipboard management

    public void Copy(EditorClipboardType type, List<string> content)
    {
        if (!Contents.ContainsKey(type))
            Contents.Add(type, new BindableList<string>());

        Contents[type] = new BindableList<string>(content);
    }
    
    public BindableList<string> Paste(EditorClipboardType type)
    {
        if (Contents.TryGetValue(type, out var content))
            return content;
        
        return new BindableList<string>();
    }

    #endregion

    #region Undo / Redo

    public override void BeginChange()
    {
        ensureStateSaved();
        
        base.BeginChange();
    }

    private void ensureStateSaved()
    {
        if (savedStates.Count == 0)
            SaveState();
    }

    protected override void UpdateState()
    {
        if (isRestoring)
            return;

        using (var stream = new MemoryStream())
        {
            writeCurrentStateToStream(stream);
            var newState = stream.ToArray();
            
            if (savedStates.Count > 0 && newState.SequenceEqual(savedStates[currentState]))
                return;
            
            if (currentState < savedStates.Count - 1)
                savedStates.RemoveRange(currentState + 1, savedStates.Count - currentState - 1);
            
            if (savedStates.Count > MAX_SAVED_STATES)
                savedStates.RemoveAt(0);
            
            savedStates.Add(newState);

            currentState = savedStates.Count - 1;
            
            OnStateChange?.Invoke();
            updateBindables();
        }
    }

    public void RestoreState(int direction)
    {
        if (TransactionInProgress)
            return;
        
        if (savedStates.Count == 0)
            return;

        var newState = Math.Clamp(currentState + direction, 0, savedStates.Count - 1);
        if (currentState == newState)
            return;

        isRestoring = true;
     
        patcher.Patch(savedStates[currentState], savedStates[newState]);
        currentState = newState;
        
        isRestoring = false;
        
        OnStateChange?.Invoke();
        updateBindables();
    }

    private void writeCurrentStateToStream(Stream stream)
    {
        var encoder = new ChartEncoder();
        encoder.Encode((Chart) editorChart.PlayableChart, stream);
    }

    private void updateBindables()
    {
        CanUndo.Value = savedStates.Count > 0 && currentState > 0;
        CanRedo.Value = currentState < savedStates.Count - 1;
    }

    #endregion
}

public enum EditorClipboardType
{
    Note
}