using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Game.Screens.Edit.Compose.Components;

public partial class EditorRadioButtonCollection : CompositeDrawable
{
    private IReadOnlyList<RadioButton> items = Array.Empty<RadioButton>();

    public IReadOnlyList<RadioButton> Items
    {
        get => items;
        set
        {
            if (ReferenceEquals(items, value))
                return;

            items = value;
            
            buttonContainer.Clear();
            items.ForEach(addButton);
        }
    }

    private readonly FillFlowContainer<EditorRadioButton> buttonContainer;

    public EditorRadioButtonCollection()
    {
        AutoSizeAxes = Axes.Both;

        InternalChild = buttonContainer = new FillFlowContainer<EditorRadioButton>
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 4)
        };
    }

    private RadioButton? currentlySelected;
    
    private void addButton(RadioButton button)
    {
        button.Selected.ValueChanged += selected =>
        {
            if (selected.NewValue)
            {
                currentlySelected?.Deselect();
                currentlySelected = button;
            }
            else
                currentlySelected = null;
        };
        
        buttonContainer.Add(new EditorRadioButton(button));
    }
}
