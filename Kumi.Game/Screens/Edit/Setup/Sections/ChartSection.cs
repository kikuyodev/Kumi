using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Models;
using Kumi.Game.Screens.Edit.Components;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Screens.Edit.Setup.Sections;

public partial class ChartSection : SetupSection
{
    private KumiTextBox creatorTextBox = null!;
    private KumiTextBox difficultyTextBox = null!;
    private EditorSliderBar<float> scrollSpeedSlider = null!;

    private BindableFloat scrollSpeed = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        scrollSpeed = new BindableFloat
        {
            Value = Chart.ChartInfo.InitialScrollSpeed,
            MinValue = 0.5f,
            MaxValue = 2.5f,
            Precision = 0.1f
        };

        AddRange(new[]
        {
            CreateLabelledComponent(creatorTextBox = createDefaultTextBox<KumiTextBox>(Chart.Metadata.Creator?.Username ?? string.Empty), "Creator", f =>
            {
                f.AddText("Separate creators should by separated by commas (,).");
            }),
            CreateLabelledComponent(difficultyTextBox = createDefaultTextBox<KumiTextBox>(Chart.ChartInfo.DifficultyName), "Difficulty name"),
            CreateLabelledComponent(scrollSpeedSlider = new EditorSliderBar<float>
            {
                RelativeSizeAxes = Axes.X,
                Current = { BindTarget = scrollSpeed },
                BarHeight = 30,
                BackgroundColour = Colours.Gray(0.1f),
                BarColour = Colours.BLUE_ACCENT
            }, "Scroll speed", f =>
            {
                f.AddText("Initial scroll speed, every inherited timing point will be relative to this value.");
            })
        });
        
        foreach (var textBox in Children.OfType<KumiTextBox>())
            textBox.OnCommit += onCommit;
        
        scrollSpeedSlider.Current.BindValueChanged(onScrollSpeedChanged, true);
    }

    private T createDefaultTextBox<T>(string text)
        where T : KumiTextBox, new()
    {
        return new T
        {
            RelativeSizeAxes = Axes.X,
            Height = 30,
            Text = text,
            TabbableContentContainer = this
        };
    }

    private void onCommit(TextBox sender, bool newText)
    {
        if (!newText)
            return;

        Scheduler.AddOnce(updateMetadata);
    }
    
    private void onScrollSpeedChanged(ValueChangedEvent<float> e)
    {
        Scheduler.AddOnce(updateMetadata);
    }

    private void updateMetadata()
    {
        Chart.Metadata.Creator ??= new RealmAccount();
        Chart.Metadata.Creator.Username = creatorTextBox.Text;

        Chart.ChartInfo.DifficultyName = difficultyTextBox.Text;
        
        Chart.ChartInfo.InitialScrollSpeed = scrollSpeed.Value;

        Chart.SaveState();
    }
}
