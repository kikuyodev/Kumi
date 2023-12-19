using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Kumi.Game.Screens.Edit.Setup.Sections;

public partial class SetupSection : FillFlowContainer
{
    [Resolved]
    protected EditorChart Chart { get; private set; } = null!;
    
    public SetupSection()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Spacing = new Vector2(0, 12);

        AutoSizeDuration = 200;
        AutoSizeEasing = Easing.OutQuint;
    }
    
    protected Drawable CreateLabelledComponent(Drawable component, string title, Action<TextFlowContainer>? description = null)
    {
        var textFlow = new TextFlowContainer(c =>
        {
            c.Font = KumiFonts.GetFont(size: 12);
            c.Colour = Colours.GRAY_6;
        })
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
        };
        
        var container = new GridContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            ColumnDimensions = new []
            {
                new Dimension(),
                new Dimension()
            },
            RowDimensions = new[]
            {
                new Dimension(GridSizeMode.AutoSize)
            },
            Content = new[]
            {
                new[]
                {
                    component,
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 4),
                        Padding = new MarginPadding
                        {
                            Left = 12
                        },
                        Children = new Drawable[]
                        {
                            new SpriteText
                            {
                                Text = title,
                                Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                                Colour = Colours.GRAY_C
                            },
                            textFlow
                        }
                    }
                }
            }
        };

        description?.Invoke(textFlow);

        return container;
    }
}
