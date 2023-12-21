using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace Kumi.Game.Screens.Edit.Timing.Configuration;

public partial class UninheritedSection : ConfigurationSection
{
    protected override LocalisableString Title => "Uninherited settings";

    private UninheritedTimingPoint uninheritedPoint => (UninheritedTimingPoint)Point;

    private readonly Bindable<float> bpmBindable;
    private readonly Bindable<TimeSignature> timeSignatureBindable;
    
    public UninheritedSection(TimingPoint point)
        : base(point)
    {
        bpmBindable = uninheritedPoint.GetBindableBPM().GetBoundCopy();
        timeSignatureBindable = uninheritedPoint.TimeSignatureBindable.GetBoundCopy();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        bpmBindable.BindValueChanged(_ => saveSettings());
        timeSignatureBindable.BindValueChanged(_ => saveSettings());
    }

    protected override Drawable[] CreateContent()
        => new[]
        {
            createLabelledComponent(new BPMTextBox
            {
                Width = 90,
                Height = 24,
                BpmBindable = bpmBindable,
                BackgroundColour = Colours.Gray(0.05f),
                CommitOnFocusLost = true,
            }, "BPM"),
            createLabelledComponent(new TimeSignatureTextBox
            {
                TimeSignatureBindable = timeSignatureBindable
            }, "Time signature")
        };
    
    private void saveSettings()
    {
        HistoryHandler?.SaveState();
    }

    private Drawable createLabelledComponent(Drawable component, LocalisableString label)
        => new Container
        {
            AutoSizeAxes = Axes.Both,
            Child = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Spacing = new(4, 0),
                Children = new[]
                {
                    component.With(d =>
                    {
                        d.Anchor = Anchor.CentreLeft;
                        d.Origin = Anchor.CentreLeft;
                    }),
                    new SpriteText
                    {
                        Text = label,
                        Font = KumiFonts.GetFont(size: 12),
                        Colour = Colours.YELLOW_ACCENT_LIGHT,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    }
                }
            }
        };
}