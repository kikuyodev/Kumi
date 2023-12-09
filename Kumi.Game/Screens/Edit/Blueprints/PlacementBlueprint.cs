using Kumi.Game.Charts.Objects;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Edit.Blueprints;

public abstract partial class PlacementBlueprint : CompositeDrawable
{
    public PlacementState PlacementActive { get; private set; }

    public readonly Note Note;

    [Resolved]
    private EditorClock editorClock { get; set; } = null!;
    
    [Resolved]
    private EditorChart chart { get; set; } = null!;

    [Resolved]
    private IPlacementHandler placementHandler { get; set; } = null!;
    
    private Bindable<double> startTimeBindable = null!;

    protected PlacementBlueprint(Note note)
    {
        Note = note;

        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        startTimeBindable = Note.StartTimeBindable.GetBoundCopy();
        startTimeBindable.BindValueChanged(v =>
        {
            Note.StartTime = v.NewValue;
        });
    }

    protected void BeginPlacement(bool commitStart = false)
    {
        placementHandler.BeginPlacement(Note);
        if (commitStart)
            PlacementActive = PlacementState.Active;
    }

    public void EndPlacement(bool commit)
    {
        switch (PlacementActive)
        {
            case PlacementState.Finished:
                return;
            
            case PlacementState.Waiting:
                BeginPlacement();
                break;
        }
        
        placementHandler.EndPlacement(Note, commit);
        PlacementActive = PlacementState.Finished;
    }

    public virtual void UpdateTimeAndPosition(double? time)
    {
        if (PlacementActive == PlacementState.Waiting)
        {
            Note.StartTime = time ?? editorClock.CurrentTime;
        }
    }
}

public enum PlacementState
{
    Waiting,
    Active,
    Finished
}
