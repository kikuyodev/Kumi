using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Tools;
using osu.Framework.Allocation;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osuTK;

namespace Kumi.Game.Screens.Edit.Compose;

public partial class ComposeBlueprintContainer : EditorBlueprintContainer
{
    private Container<PlacementBlueprint> placementBlueprintContainer = null!;
    
    private InputManager inputManager = null!;
    
    private NoteCompositionTool? currentTool;

    public NoteCompositionTool? CurrentTool
    {
        get => currentTool;
        set
        {
            if (currentTool == value)
                return;

            currentTool = value;
            commitIfPlacementActive();
        }
    }

    public PlacementBlueprint? CurrentPlacement { get; private set; }
    
    [Resolved]
    private EditorChart editorChart { get; set; } = null!;
    
    [Resolved]
    private EditorScreenWithTimeline? editorScreen { get; set; }
    
    [Resolved]
    private BindableBeatDivisor beatDivisor { get; set; } = null!;

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => editorScreen?.MainContent.ReceivePositionalInputAt(screenSpacePos) ?? base.ReceivePositionalInputAt(screenSpacePos);

    [BackgroundDependencyLoader]
    private void load()
    {
        AddInternal(placementBlueprintContainer = new Container<PlacementBlueprint>
        {
            RelativeSizeAxes = Axes.Both
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        inputManager = GetContainingInputManager();
        editorChart.NoteAdded += noteAdded;
    }

    protected override SelectionBlueprint<Note>? CreateBlueprintFor(Note note)
    {
        var drawable = Composer?.Notes.FirstOrDefault(d => d.Note == note);

        if (drawable == null)
            return null;

        return CreateNoteBlueprintFor(note)?.With(b => b.DrawableNote = drawable);
    }

    public virtual NoteSelectionBlueprint? CreateNoteBlueprintFor(Note note)
        => note switch
        {
            DrumHit drumHit => drumHit.Flags.Value.HasFlagFast(NoteFlags.Big)
                ? new BigDrumNoteSelectionBlueprint(note)
                : new DrumNoteSelectionBlueprint(note),
            _ => null
        };

    protected override void Update()
    {
        base.Update();

        if (CurrentPlacement != null)
        {
            switch (CurrentPlacement.PlacementActive)
            {
                case PlacementState.Waiting:
                    if (!Composer?.CursorInPlacementArea ?? false)
                        removePlacement();
                    break;
                
                case PlacementState.Finished:
                    removePlacement();
                    break;
            }
        }

        if (Composer?.CursorInPlacementArea ?? false)
            ensurePlacementCreated();
        
        if (CurrentPlacement != null)
            updatePlacementPosition();
    }

    private void refreshTool()
    {
        removePlacement();
        ensurePlacementCreated();
    }

    private void updatePlacementPosition()
    {
        var targetTime = Composer!.Playfield!.TimeAtScreenSpacePosition(inputManager.CurrentState.Mouse.Position);
        targetTime = Composer.Playfield.SnapTime(targetTime, beatDivisor.Value);
        
        CurrentPlacement?.UpdateTimeAndPosition(targetTime);
    }

    private void noteAdded(Note note)
    {
        refreshTool();
    }

    private void ensurePlacementCreated()
    {
        if (CurrentPlacement != null)
            return;

        var blueprint = CurrentTool?.CreatePlacementBlueprint();
        
        if (blueprint == null)
            return;

        placementBlueprintContainer.Child = CurrentPlacement = blueprint;
        
        updatePlacementPosition();
    }

    private void commitIfPlacementActive()
    {
        CurrentPlacement?.EndPlacement(CurrentPlacement.PlacementActive == PlacementState.Active);
        removePlacement();
    }

    private void removePlacement()
    {
        CurrentPlacement?.EndPlacement(false);
        CurrentPlacement?.Expire();
        CurrentPlacement = null;
    }
}
