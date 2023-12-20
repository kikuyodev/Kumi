using Kumi.Game.Bindables;
using Kumi.Game.Charts.Objects.Windows;

namespace Kumi.Game.Charts.Objects;

/// <summary>
/// A bar line, which is to denote a measure.
/// </summary>
// For sake of simplicity, we're just going to inherit from Note and treat it as such,
// though not implementing any judgement logic for it.
public class BarLine : Note
{
    public LazyBindable<bool> Major { get; } = new LazyBindable<bool>();
    
    public BarLine()
        : this(0)
    {
    }
    
    public BarLine(float startTime)
        : base(startTime)
    {
        Windows = new NoteWindows();
    }
}
