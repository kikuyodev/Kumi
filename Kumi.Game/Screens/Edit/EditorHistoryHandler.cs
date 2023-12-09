using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace Kumi.Game.Screens.Edit;

/// <summary>
/// A class that handles copying and pasting of notes, timing points, and other objects;
/// as well as undoing and redoing actions.
/// </summary>
public partial class EditorHistoryHandler : Component
{
    /// <summary>
    /// A dictionary of the contents of the clipboard.
    /// </summary>
    public Dictionary<EditorClipboardType, Bindable<List<string>>> Contents { get; } = new();

    public EditorHistoryHandler(EditorChart editorChart)
    {
        this.editorChart = editorChart;
        
        Contents
           .Add(EditorClipboardType.Note, new Bindable<List<string>>());
    }
    
    private readonly EditorChart editorChart;
    
    public void Copy(EditorClipboardType type, List<string> content)
    {
        if (!Contents.ContainsKey(type))
            Contents.Add(type, new Bindable<List<string>>(new List<string>()));

        Contents[type].Value = content;
    }
    
    public List<string> Paste(EditorClipboardType type)
    {
        if (Contents.ContainsKey(type))
            return Contents[type].Value;
        
        return new List<string>();
    }
    
    public void BindValueChanged(EditorClipboardType type, Action<ValueChangedEvent<List<string>>> action)
    {
        if (Contents.ContainsKey(type))
            Contents[type].BindValueChanged(action);
    }
}

public enum EditorClipboardType
{
    Note
}