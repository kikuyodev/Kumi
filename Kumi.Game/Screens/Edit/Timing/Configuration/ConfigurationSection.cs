using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timing.Configuration;

public abstract partial class ConfigurationSection : CompositeDrawable
{
    protected TimingPoint Point { get; set; }

    protected abstract LocalisableString Title { get; }
    
    [Resolved]
    protected EditorChart Chart { get; private set; } = null!;

    [Resolved]
    protected EditorHistoryHandler? HistoryHandler { get; private set; }
    
    protected ConfigurationSection(TimingPoint point)
    {
        Point = point;
        AutoSizeAxes = Axes.Both;
    }
    
    private FillFlowContainer content = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = content = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 8),
            Children = new Drawable[]
            {
                new SpriteText
                {
                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 12),
                    Colour = Colours.GRAY_C,
                    Text = Title
                },
            }
        };
        
        content.AddRange(CreateContent());
    }
    
    protected abstract Drawable[] CreateContent();
}
