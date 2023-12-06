using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace Kumi.Game.Screens.Edit.Compose.Components;

public partial class EditorRadioButton : Button
{
    private Container marginContainer = null!;
    private Container contentContainer = null!;
    private Drawable icon = null!;
    private Drawable text = null!;

    private Drawable background1 = null!;
    private Drawable background2 = null!;

    private RadioButton button;

    public EditorRadioButton(RadioButton button)
    {
        this.button = button;

        Action = button.Select;
        AutoSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new Container
        {
            AutoSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                marginContainer = new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    CornerRadius = 5,
                    Masking = true,
                    X = 12,
                    Children = new Drawable[]
                    {
                        background2 = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colours.Gray(0.05f)
                        }
                    }
                },
                contentContainer = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    AutoSizeDuration = 200,
                    AutoSizeEasing = Easing.OutQuint,
                    X = 12,
                    CornerRadius = 5,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        background1 = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colours.Gray(0.05f)
                        },
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(8, 0),
                            Padding = new MarginPadding(8),
                            Children = new[]
                            {
                                icon = (button.CreateIcon?.Invoke() ?? new Circle()).With(b =>
                                {
                                    b.Anchor = Anchor.CentreLeft;
                                    b.Origin = Anchor.CentreLeft;
                                    b.Size = new Vector2(16);
                                    b.Colour = Colours.GRAY_6;
                                }),
                                text = new SpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Font = KumiFonts.GetFont(weight: FontWeight.SemiBold, size: 12),
                                    Text = button.Label,
                                    Colour = Colours.GRAY_F,
                                    Alpha = 0
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        button.Selected.BindValueChanged(_ => updateSelectionState(), true);
        button.Selected.BindDisabledChanged(d => Enabled.Value = !d, true);
    }

    protected override void Update()
    {
        base.Update();
        
        marginContainer.Width = contentContainer.DrawWidth;
    }

    private void updateSelectionState()
    {
        background1.FadeColour(button.Selected.Value ? Colours.BLUE : Colours.Gray(0.05f), 200, Easing.OutQuint);
        background2.FadeColour(button.Selected.Value ? Colours.BLUE : Colours.Gray(0.05f), 200, Easing.OutQuint);
        
        icon.FadeColour(button.Selected.Value ? Colours.GRAY_F : Colours.GRAY_6, 200, Easing.OutQuint);
        text.FadeTo(button.Selected.Value ? 1 : 0, 200, Easing.OutQuint);

        marginContainer.MoveToX(button.Selected.Value ? -5 : 12, 200, Easing.OutQuint);
    }
}
