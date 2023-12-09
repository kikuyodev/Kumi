using Kumi.Game.Screens.Edit.Compose.Components;
using Kumi.Game.Tests;
using osu.Framework.Graphics;

namespace Kumi.Tests.Visual.Edit;

public partial class EditorComposeRadioButtonsTestScene : KumiTestScene
{
    public EditorComposeRadioButtonsTestScene()
    {
        EditorRadioButtonCollection collection;
        Add(collection = new EditorRadioButtonCollection
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            Items = new[]
            {
                new RadioButton("Item 1", () => { }),
                new RadioButton("Item 2", () => { }),
                new RadioButton("Item 3", () => { }),
                new RadioButton("Item 4", () => { }),
                new RadioButton("Item 5", () => { }),
            }
        });

        for (var i = 0; i < collection.Items.Count; i++)
        {
            var j = i;
            AddStep($"Select item {j + 1}", () => collection.Items[j].Select());
            AddStep($"Deselect item {j + 1}", () => collection.Items[j].Deselect());
        }
    }
}
