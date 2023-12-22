using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Overlays.Settings;

public abstract partial class SettingSection : Container
{
    protected abstract LocalisableString Header { get; }

    private FillFlowContainer content = null!;

    protected override Container<Drawable> Content => content;

    protected SettingSection()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 12),
            Children = new Drawable[]
            {
                new SpriteText
                {
                    Text = Header,
                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold)
                },
                content = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 8)
                }
            }
        };
    }
}
