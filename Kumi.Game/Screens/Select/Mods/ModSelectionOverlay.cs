using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace Kumi.Game.Screens.Select.Mods;

public partial class ModSelectionOverlay : KumiFocusedOverlayContainer
{
    public ModSelectionOverlay()
    {
        RelativeSizeAxes = Axes.Y;
        Width = 400;
        Padding = new MarginPadding(8);
    }

    private KumiTextBox searchBox = null!;
    private SearchContainer searchContainer = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.Gray(0.05f)
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new[]
                {
                    new Dimension()
                },
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize),
                    new Dimension()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Colours.Gray(0.05f)
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Padding = new MarginPadding(12),
                                    Children = new Drawable[]
                                    {
                                        searchBox = new KumiTextBox
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 30,
                                            PlaceholderText = "Search mods...",
                                            FontSize = 16,
                                        }
                                    }
                                }
                            }
                        },
                    },
                    new Drawable[]
                    {
                        new BasicScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = searchContainer = new SearchContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Child = new ModSelectorList()
                            }
                        }
                    }
                }
            }
        };

        searchBox.Current.BindValueChanged(v => searchContainer.SearchTerm = v.NewValue);
    }

    protected override void PopIn()
    {
        this.MoveToX(0, 200, Easing.OutQuint);
        this.FadeIn(200, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.MoveToX(DrawWidth, 200, Easing.OutQuint);
        this.FadeOut(200, Easing.OutQuint);
    }
}
