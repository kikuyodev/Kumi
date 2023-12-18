using System.Collections.Specialized;
using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Compose;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Blueprints;

public abstract partial class BlueprintContainer : CompositeDrawable
{
    private readonly Dictionary<Note, SelectionBlueprint<Note>> blueprintMap = new Dictionary<Note, SelectionBlueprint<Note>>();

    public Container<SelectionBlueprint<Note>> SelectionBlueprints { get; private set; } = null!;

    public SelectionHandler SelectionHandler { get; private set; } = null!;

    protected readonly BindableList<Note> SelectedItems = new BindableList<Note>();

    [Resolved]
    private ComposeScreen composer { get; set; } = null!;

    [Resolved]
    private EditorChart editorChart { get; set; } = null!;

    [Resolved]
    private BindableBeatDivisor divisor { get; set; } = null!;

    protected BlueprintContainer()
    {
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        SelectedItems.CollectionChanged += (_, args) =>
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Debug.Assert(args.NewItems != null);

                    foreach (var o in args.NewItems)
                    {
                        if (blueprintMap.TryGetValue((Note) o, out var blueprint))
                            blueprint.Select();
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    Debug.Assert(args.OldItems != null);

                    foreach (var o in args.OldItems)
                    {
                        if (blueprintMap.TryGetValue((Note) o, out var blueprint))
                            blueprint.Deselect();
                    }

                    break;
            }
        };

        SelectionHandler = CreateSelectionHandler();
        SelectionHandler.SelectedItems.BindTo(SelectedItems);

        AddRangeInternal(new Drawable[]
        {
            SelectionHandler,
            SelectionBlueprints = CreateSelectionBlueprintContainer().With(b =>
            {
                b.RelativeSizeAxes = Axes.Both;
            })
        });
    }
    
    protected virtual SelectionHandler CreateSelectionHandler()
        => new SelectionHandler();
    
    protected virtual Container<SelectionBlueprint<Note>> CreateSelectionBlueprintContainer()
        => new NoteOrderedSelectionContainer();

    protected virtual void AddBlueprintFor(Note note)
    {
        if (blueprintMap.ContainsKey(note))
            return;

        var blueprint = CreateBlueprintFor(note);
        if (blueprint == null)
            return;

        blueprintMap[note] = blueprint;

        blueprint.Selected += OnBlueprintSelected;
        blueprint.Deselected += OnBlueprintDeselected;

        SelectionBlueprints.Add(blueprint);

        if (SelectionHandler.SelectedItems.Contains(note))
            blueprint.Select();

        OnBlueprintAdded(blueprint.Item);
    }

    protected void RemoveBlueprintFor(Note note)
    {
        if (!blueprintMap.Remove(note, out var blueprint))
            return;

        blueprint.Deselect();
        blueprint.Selected -= OnBlueprintSelected;
        blueprint.Deselected -= OnBlueprintDeselected;

        SelectionBlueprints.Remove(blueprint, true);

        if (movementBlueprints?.Contains(blueprint) == true)
            finishSelectionMovement();

        OnBlueprintRemoved(blueprint.Item);
    }

    protected virtual void OnBlueprintAdded(Note note)
    {
    }

    protected virtual void OnBlueprintRemoved(Note note)
    {
    }

    protected virtual SelectionBlueprint<Note>? CreateBlueprintFor(Note note)
        => null;

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        var movementPossible = prepareSelectionMovement();

        if (performMouseDownSelection(e))
            return true;

        return e.Button == MouseButton.Left && movementPossible;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        // Push this to the next frame, as OnClick will be called before this.
        Schedule(() =>
        {
            endClickSelection(e);
            clickSelectionHandled = false;
            isDraggingBlueprint = false;
            wasDragStarted = false;
        });

        finishSelectionMovement();
    }

    private MouseButtonEvent? lastDragEvent;

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button == MouseButton.Right)
            return false;

        lastDragEvent = e;
        wasDragStarted = true;

        if (movementBlueprints != null)
        {
            BlueprintPivot = SelectionHandler.SelectedBlueprints.FirstOrDefault(b => b.IsHovered);

            isDraggingBlueprint = true;
            editorChart.BeginChange();
            return true;
        }

        return true;
    }

    protected override void OnDrag(DragEvent e)
    {
        lastDragEvent = e;
        moveCurrentSelection(e);
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        lastDragEvent = null;

        if (isDraggingBlueprint)
        {
            composer.Playfield.UpdateLifetimeRange(editorChart.SelectedNotes.ToArray());
            editorChart.EndChange();
        }
    }

    #region Selection

    protected SelectionBlueprint<Note>? BlueprintPivot { get; private set; }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button == MouseButton.Right)
            return false;

        BlueprintPivot = SelectionHandler.SelectedBlueprints.FirstOrDefault(b => b.IsHovered);

        if (endClickSelection(e) || BlueprintPivot != null)
            return true;

        DeselectAll();
        return true;
    }

    protected void DeselectAll()
        => SelectedItems.Clear();

    protected virtual void OnBlueprintSelected(SelectionBlueprint<Note> blueprint)
    {
        SelectionHandler.HandleSelected(blueprint);
        SelectionBlueprints.ChangeChildDepth(blueprint, 1);
    }

    protected virtual void OnBlueprintDeselected(SelectionBlueprint<Note> blueprint)
    {
        SelectionBlueprints.ChangeChildDepth(blueprint, 0);
        SelectionHandler.HandleDeselected(blueprint);
    }

    private bool clickSelectionHandled;

    private bool selectedBlueprintAlreadySelectedOnMouseDown;

    private bool endClickSelection(MouseButtonEvent e)
    {
        if (clickSelectionHandled || isDraggingBlueprint) return true;
        if (e.Button != MouseButton.Left) return false;

        if (e.ShiftPressed)
        {
            foreach (var blueprint in SelectionBlueprints.AliveChildren.Where(b => b.IsHovered).OrderByDescending(b => b.IsSelected))
                return clickSelectionHandled = SelectionHandler.MouseUpSelectionRequested(blueprint, e);

            return false;
        }

        if (!wasDragStarted && selectedBlueprintAlreadySelectedOnMouseDown && SelectedItems.Count == 1)
        {
            var cyclingBlueprints = blueprintMap.Values.Reverse();
            cyclingBlueprints = cyclingBlueprints.SkipWhile(b => !b.IsSelected).Skip(1);
            cyclingBlueprints = cyclingBlueprints.Concat(blueprintMap.Values.Reverse().TakeWhile(b => !b.IsSelected));

            foreach (var blueprint in cyclingBlueprints)
            {
                if (!blueprint.IsHovered) continue;

                return clickSelectionHandled = SelectionHandler.MouseDownSelectionRequested(blueprint, e);
            }
        }

        return false;
    }

    private bool performMouseDownSelection(MouseButtonEvent e)
    {
        foreach (var blueprint in SelectionBlueprints.AliveChildren.Where(b => b.IsSelected))
        {
            if (runForBlueprint(blueprint))
                return true;
        }

        foreach (var blueprint in SelectionBlueprints.AliveChildren.Reverse())
        {
            if (runForBlueprint(blueprint))
                return true;
        }

        return false;

        bool runForBlueprint(SelectionBlueprint<Note> blueprint)
        {
            if (!blueprint.IsHovered)
                return false;

            selectedBlueprintAlreadySelectedOnMouseDown = blueprint.State == SelectionState.Selected;
            clickSelectionHandled = SelectionHandler.MouseDownSelectionRequested(blueprint, e);
            return true;
        }
    }

    #endregion

    #region Movement

    private SelectionBlueprint<Note>[]? movementBlueprints;

    private bool isDraggingBlueprint;

    private bool wasDragStarted;

    private bool prepareSelectionMovement()
    {
        if (!SelectionHandler.SelectedBlueprints.Any())
            return false;

        if (!clickSelectionHandled && !SelectionHandler.SelectedBlueprints.Any(b => b.IsHovered))
            return false;

        movementBlueprints = SelectionHandler.SelectedBlueprints.ToArray();
        return true;
    }

    private bool moveCurrentSelection(DragEvent e)
    {
        if (movementBlueprints == null)
            return false;

        var targetTime = composer.Playfield!.TimeAtScreenSpacePosition(e.ScreenSpaceMousePosition);
        targetTime = composer.Playfield.SnapTime(targetTime, divisor.Value);
        var offset = targetTime - BlueprintPivot?.Item.StartTime ?? targetTime;

        editorChart.PerformOnSelection(n =>
        {
            n.StartTime += offset;

            if (n is IHasEndTime endTime)
                endTime.EndTime += offset;

            editorChart.Update(n);
        });

        return true;
    }

    private bool finishSelectionMovement()
    {
        if (movementBlueprints == null)
            return false;

        movementBlueprints = null;
        return true;
    }

    #endregion

}
