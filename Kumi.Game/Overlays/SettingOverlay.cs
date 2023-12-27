using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Overlays.Settings;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace Kumi.Game.Overlays;

public partial class SettingOverlay : KumiFocusedOverlayContainer
{
    public const float TEXT_BOX_WIDTH = 300;
    
    protected override bool StartHidden => true;

    private KumiTabControl<SettingSection> sectionTabControl = null!;
    private KumiTextBox searchTextBox = null!;

    public SettingOverlay()
    {
        Width = 1000;
        Height = 500;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
    }

    private SearchContainer screenContainer = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = 5,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.05f)
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding
                    {
                        Horizontal = 20,
                        Vertical = 12
                    },
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        RowDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize),
                            new Dimension(GridSizeMode.Absolute, 24),
                            new Dimension()
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new GridContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    ColumnDimensions = new[]
                                    {
                                        new Dimension(),
                                        new Dimension(GridSizeMode.Absolute, TEXT_BOX_WIDTH)
                                    },
                                    Content = new[]
                                    {
                                        new Drawable[]
                                        {
                                            sectionTabControl = new KumiTabControl<SettingSection>
                                            {
                                                RelativeSizeAxes = Axes.None,
                                                Width = 300,
                                                Height = 24,
                                                Items = new[]
                                                {
                                                    SettingSection.Graphics,
                                                    SettingSection.Audio
                                                },
                                                Current = { Value = SettingSection.Graphics }
                                            },
                                            searchTextBox = new KumiTextBox
                                            {
                                                Anchor = Anchor.TopRight,
                                                Origin = Anchor.TopRight,
                                                RelativeSizeAxes = Axes.X,
                                                Height = 24,
                                                PlaceholderText = "Search options..."
                                            }
                                        }
                                    }
                                }
                            },
                            new[]
                            {
                                Empty()
                            },
                            new Drawable[]
                            {
                                new KumiScrollContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Child = screenContainer = new SearchContainer
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        sectionTabControl.Current.BindValueChanged(onScreenChanged, true);
        searchTextBox.Current.BindValueChanged(v =>
        {
            screenContainer.SearchTerm = v.NewValue;
        }, true);
    }

    private void onScreenChanged(ValueChangedEvent<SettingSection> screen)
    {
        screenContainer.Clear();

        switch (screen.NewValue)
        {
            case SettingSection.Graphics:
                screenContainer.Add(new GraphicsSettings());
                break;

            case SettingSection.Audio:
                screenContainer.Add(new AudioSettings());
                break;
        }
    }

    protected override void PopIn()
    {
        this.FadeIn(200, Easing.OutQuint);
        this.ScaleTo(1f, 200, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.FadeOut(200, Easing.OutQuint);
        this.ScaleTo(0.9f, 200, Easing.OutQuint);
    }
}

public enum SettingSection
{
    Graphics,
    Audio
}
