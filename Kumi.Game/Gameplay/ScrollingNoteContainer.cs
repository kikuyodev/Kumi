using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay.Algorithms;
using Kumi.Game.Gameplay.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Layout;
using osuTK;

namespace Kumi.Game.Gameplay;

public partial class ScrollingNoteContainer : Container<DrawableNote>
{
    public readonly IBindable<double> TimeRange = new BindableDouble();
    public readonly IBindable<IScrollAlgorithm> Algorithm = new Bindable<IScrollAlgorithm>();
    
    private readonly HashSet<DrawableNote> layoutComputed = new HashSet<DrawableNote>();
    private readonly LayoutValue layoutCache = new LayoutValue(Invalidation.RequiredParentSizeToFit | Invalidation.DrawInfo);
    
    public ScrollingNoteContainer()
    {
        AddLayout(layoutCache);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        TimeRange.ValueChanged += _ => layoutCache.Invalidate();
        Algorithm.ValueChanged += _ => layoutCache.Invalidate();
    }

    public override void Add(DrawableNote drawable)
    {
        if (IsLoaded)
            setLifetimeStart(drawable);

        base.Add(drawable);
    }

    public override bool Remove(DrawableNote drawable, bool disposeImmediately)
    {
        layoutComputed.Remove(drawable);
        return base.Remove(drawable, disposeImmediately);
    }

    protected override void Update()
    {
        base.Update();
        
        if (layoutCache.IsValid)
            return;
        
        layoutComputed.Clear();

        foreach (var child in Children)
            setLifetimeStart(child);
        
        layoutCache.Validate();
    }

    protected override void UpdateAfterChildrenLife()
    {
        base.UpdateAfterChildrenLife();

        foreach (var child in Children)
        {
            if (child == null)
                continue;
            
            updatePosition(child, Time.Current);
            
            if (layoutComputed.Contains(child))
                continue;

            updateLayout(child);

            layoutComputed.Add(child);  
        }
    }

    private void updateLayout(DrawableNote note)
    {
        if (note.Note is IHasEndTime endTime)
            note.Width = lengthAtTime(note.Note.StartTime, endTime.EndTime);
        
        updatePosition(note, note.Note.StartTime);
        setLifetimeStart(note);
    }

    public double TimeAtScreenSpacePosition(Vector2 screenSpacePosition)
    {
        var pos = ToLocalSpace(screenSpacePosition);
        return timeAtPosition(pos.X, Time.Current);
    }
    
    public Vector2 ScreenSpacePositionAtTime(double time)
    {
        var position = positionAtTime(time, Time.Current);
        return ToScreenSpace(new Vector2(position, 0));
    }

    private RectangleF getBoundingBox() => new RectangleF().Inflate(100);

    public double ComputeLifetimeStart(DrawableNote note)
    {
        var boundingBox = getBoundingBox();
        var startOffset = -boundingBox.Left;

        return Algorithm.Value.GetDrawableStartTime(note.Note.StartTime, startOffset, TimeRange.Value, DrawWidth);
    }
    
    private void setLifetimeStart(DrawableNote note)
    {
        var computedStartTime = ComputeLifetimeStart(note);
        note.LifetimeStart = Math.Min(note.Note.StartTime - note.Note.Windows.WindowFor(NoteHitResult.Bad), computedStartTime);
    }

    private void updatePosition(DrawableNote note, double currentTime)
    {
        var position = positionAtTime(note.Note.StartTime, currentTime);
        note.X = position;
    }
    
    private float lengthAtTime(double startTime, double endTime)
        => Algorithm.Value.GetLength(startTime, endTime, TimeRange.Value, DrawWidth);

    private float positionAtTime(double time, double currentTime)
        => Algorithm.Value.PositionAt(time, currentTime, TimeRange.Value, DrawWidth);

    private double timeAtPosition(float localPosition, double currentTime)
        => Algorithm.Value.TimeAt(localPosition, currentTime, TimeRange.Value, DrawWidth);
}
